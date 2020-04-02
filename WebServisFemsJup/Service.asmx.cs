using Newtonsoft.Json;
using SimpleCrypto;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;


namespace WebServisFemsJup
{
    /// <summary>
    /// Descripción breve de Service
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class Service : System.Web.Services.WebService
    {
        DB_A54C28_alexander14Entities1 bd;
        string json;
        HttpContext con;
        //Objetos
        usuario u = new usuario();
        persona p = new persona();
        calificacion cal = new calificacion();
        insidencia ins = new insidencia();
        categoriaT caT = new categoriaT();
        actividade act = new actividade();
        solicitud sol = new solicitud();
        publicacion pub = new publicacion();
        actividadesPublicacion a_p = new actividadesPublicacion();
        perfil perf = new perfil();
        elemento el = new elemento();
        elementopermitido el_permi = new elementopermitido();
        permiso per = new permiso();
        elementopermiso el_per = new elementopermiso();
        modulo mod = new modulo();
        string passEncryptada;
        string salt;

        ICryptoService cryptoService = new PBKDF2();

        public Service()
        {
            bd = new DB_A54C28_alexander14Entities1();
            con = HttpContext.Current;
            con.Response.ContentType = "application/json";
            json = "";
        }
 //{ Generales --------------------------------------------------------------------------------------------------------------------------
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Login(string email, string pass, int perfil)
        {
            var list = bd.usuarios.Where(x => x.email == email && x.idperfil == perfil).FirstOrDefault();
            if (list != null)
            {
                passEncryptada = cryptoService.Compute(pass, list.salt);
                if (cryptoService.Compare(list.pass, passEncryptada))
                {
                    var lista = (from us in bd.usuarios
                                 join pe in bd.personas on us.idpersona equals pe.id
                                 where us.id == list.id
                                 select new
                                 {
                                     id = us.id,
                                     idperfil = us.idperfil,
                                     idpersona = us.idpersona,
                                     nombre = pe.nombre,
                                     apellido = pe.apellido,
                                     telefono = pe.telefono,
                                     sexo = pe.sexo,
                                     curp = pe.curp,
                                     fechanacimiento = pe.fechanacimiento,
                                     longi = pe.@long,
                                     lat = pe.lat,
                                     idinteres = pe.idinteres,
                                     fotoperfil = pe.fotoperfil
                                 }).ToList();
                    json = JsonConvert.SerializeObject(lista);
                }
                else
                    json = JsonConvert.SerializeObject("[{ mensaje : '1' }]"); //La contraseña es incorrecta

            }
            else
                json = JsonConvert.SerializeObject("[{mensaje : '0'}]");// el usuario no existe u el perfil es incorrecto
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetCategorias()
        {
            var list = (from cat in bd.categoriaTs
                        select new
                        {
                            id = cat.id,
                            nombre = cat.nombre,
                            icono = cat.icono,
                        }).ToList();
            json = JsonConvert.SerializeObject(list);
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetActividades(int id)
        {
            var list = (from cat in bd.categoriaTs
                        join act in bd.actividades on cat.id equals act.idcategorias
                        where cat.id == id
                        select new
                        {
                            actividad = act.nombre
                            /*bd.actividades.Where(x => x.idcategorias == id).Select(x => new {
                                x.nombre
                            })*/
                        });
            json = JsonConvert.SerializeObject(list);
            con.Response.Write(json);
            con.Response.End();

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddUsuario( string email, string pass, int idperfil, string nombre,string apellido,string telefono, string sexo, string curp, string fechanacimiento, string fotoperfil, string longi, string lat, int idinteres )
        {
            salt = cryptoService.GenerateSalt();
            passEncryptada = cryptoService.Compute(pass);
            {
                p.nombre = nombre;
                p.apellido = apellido;
                p.telefono = telefono;
                p.sexo = sexo;
                p.curp = curp;
                p.fechanacimiento = fechanacimiento;
                p.fotoperfil = fotoperfil;
                p.@long = longi;
                p.lat = lat;
                p.idinteres = idinteres;
                u.email = email;
                u.pass = passEncryptada;
                u.salt = salt;
                u.idperfil = idperfil;
                u.estatus = 3;
                cal.puntuacion = 5;
                u.calificacions.Add(cal);
                p.usuarios.Add(u);
                bd.personas.Add(p);
            }
            if (bd.SaveChanges() > 0)
                json = u.id.ToString();
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void validarEmail(string email)
        {
            var lista = bd.usuarios.Where(b => b.email == email.TrimStart().TrimEnd()).ToList();
            if (lista.Count() > 0)
            {
                json = JsonConvert.SerializeObject(1);
            }
            else
            {
                json = JsonConvert.SerializeObject(2);
            }
            con.Response.Write(json);
            con.Response.End();
        }

// Aplicaciones --------------------------------------------------------------------------------------------------------------------------

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_GetPersona(int id)
        {
            var lista = (from pe in bd.personas
                         join us in bd.usuarios on pe.id equals us.idpersona
                         join cal in bd.scores on us.id equals cal.id
                         where us.id == id
                         select new
                         {
                             nombre = pe.nombre,
                             apellido = pe.apellido,
                             telefono = pe.telefono,
                             sexo = pe.sexo,
                             fotoperfil = pe.fotoperfil,
                             score = cal.calificacion

                         }).ToList();
            json = JsonConvert.SerializeObject(lista);
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_GeTPublicacion(string id)
        {
            var list = (from pub in bd.publicacions
                        join c in bd.categoriaTs on pub.idcategorias equals c.id
                        join us in bd.usuarios on pub.idusuario equals us.id
                        join pe in bd.personas on us.idpersona equals pe.id
                        join cal in bd.scores on us.idpersona equals cal.id
                        where c.id.ToString() ==id && pub.estatus == 1 //aprobado
                        select new
                        {
                            idpu = pub.id,
                            Titulo = pub.titulo,
                            descripcion = pub.descripcion,
                            fecha = pub.fecha,
                            tarifa = pub.tarifa,
                            extra = pub.extra,
                            dispo = pub.dispo,
                            lat = pub.lat,
                            longi = pub.@long,
                            empleada = pe.nombre + " " + pe.apellido,
                            actividad = bd.actividades.Join(bd.actividadesPublicacions, a => a.id, z => z.idactivid, (a, z) => new { a = a, z = z }).Where(x => x.z.idpublicacion == pub.id).Select(ac => new {
                                ac.a.nombre
                            }),
                            interes = c.id,
                            icono = c.icono,
                            radio = pub.radio,
                            categoria= c.nombre,
                            fotoEmpleada = pe.fotoperfil,
                            score = cal.calificacion,
                            telefono = pe.telefono

                        });
            json = JsonConvert.SerializeObject(list);
            con.Response.Write(json);
            con.Response.End();
        }
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_GeTPublicacionPorUser(int id)
        {
            var list = (from pub in bd.publicacions
                        join c in bd.categoriaTs on pub.idcategorias equals c.id
                        join us in bd.usuarios on pub.idusuario equals us.id
                        join pe in bd.personas on us.idpersona equals pe.id
                        where pub.idusuario == id
                        select new
                        {
                            idpublicacion = pub.id,
                            idusuario = pub.idusuario,
                            Titulo = pub.titulo,
                            descripcion = pub.descripcion,
                            fecha = pub.fecha,
                            tarifa = pub.tarifa,
                            extra = pub.extra,
                            dispo = pub.dispo,
                            lat = pub.lat,
                            longi = pub.@long,
                            empleada = pe.nombre + " " + pe.apellido,
                            actividad = bd.actividades.Join(bd.actividadesPublicacions, a => a.id, z => z.idactivid, (a, z) => new { a = a, z = z }).Where(x => x.z.idpublicacion == pub.id).Select(ac => new {
                              ac.a.nombre
                            }),
                            interes = c.id,
                            icono = c.icono,
                            radio = pub.radio

                        });
            json = JsonConvert.SerializeObject(list);
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_Addcalificacion(int idusuario, int puntuacion, string comentario)
        {
            cal.idusuario = idusuario;
            cal.puntuacion = puntuacion;
            cal.comentarios = comentario;
            bd.calificacions.Add(cal);
            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_Addinsidencia(string mensaje, int idusuario, int idusreportado, int estatus, int tipo, string fecha)
        {
            ins.idusuario = idusuario;
            ins.idusreportado = idusreportado;
            ins.mensaje = mensaje;
            ins.estatus = estatus;
            ins.fecha = Convert.ToDateTime(fecha);
            bd.insidencias.Add(ins);
            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_Addpublicacion(int idcategorias, int idusuario, string descripcion, string fecha, int tarifa, string extra, int estatus, string dispo, string longi, string lat, string titulo, string radio)
        {
            pub.idcategorias = idcategorias;
            pub.idusuario = idusuario;
            pub.descripcion = descripcion;
            pub.fecha = Convert.ToDateTime(fecha);
            pub.tarifa = tarifa;
            pub.extra = extra;
            pub.estatus = estatus;
            pub.dispo = dispo;
            pub.@long = longi;
            pub.lat = lat;
            pub.titulo = titulo;
            pub.radio = radio;
            bd.publicacions.Add(pub);
            int res = bd.SaveChanges();
            if (res > 0)
                json = "1";
            else
                json = "0";

            con.Response.Write(json);
            con.Response.End();
        }
        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_AddSolicitud(int idusuario, int idpublicacion, string f_citar, string f_trabajo, string descripcion, string @long, string lat)
        {
            sol.idusuario = idusuario;
            sol.idpublicacion = idpublicacion;
            sol.estatus = 1;
            sol.f_citar = Convert.ToDateTime(f_citar);
            sol.f_trabajo = Convert.ToDateTime(f_trabajo);
            sol.descripcion = descripcion;
            sol.@long = @long;
            sol.lat = lat;
            bd.solicituds.Add(sol);


            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_upPersona(int id, string email, string pass, string nombre, string apellido, string telefono, string sexo, string curo, string fechanacimiento, string fotoperfil, string @long, string lat, int idinteres, string documento)
        {

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_UploadFile()//Subir imagen desde android al servidor
        {
            var request = HttpContext.Current.Request;
            if (request != null)
            {
                var photo = request.Files["file"];
                if (photo != null)
                {
                    string url = "http://alexander14-001-site1.dtempurl.com/image/";
                    string respuesta = url + photo.FileName;
                    photo.SaveAs(HttpContext.Current.Server.MapPath("image/" + photo.FileName));
                    json = JsonConvert.SerializeObject(respuesta);
                    con.Response.Write(json);
                }
                else
                {
                    json = JsonConvert.SerializeObject("No hay nada");
                    con.Response.Write(json);
                }

            }
            else
            {
                json = JsonConvert.SerializeObject("NO");
                con.Response.Write(json);
            }
            con.Response.End();

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_GetSolicitudes(int idpublic)
        {
            var query = (from s in bd.solicituds
                         join p in bd.publicacions on s.idpublicacion equals p.id
                         join u in bd.usuarios on s.idusuario equals u.id
                         join per in bd.personas on u.idpersona equals per.id
                         where p.id == idpublic
                         select new
                         {
                             idsoli = s.id,
                             idusuario=s.idusuario,
                             titulo = p.titulo,
                             descripcion = p.descripcion,
                             nombre = per.nombre,
                             apellido = per.apellido,
                             fecha = s.fecha,
                             fecha_cita = s.f_citar,
                             fecha_tra = s.f_trabajo,
                             estatus = s.estatus,
                             log = s.@long,
                             lat = s.lat
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_uploadfotoperfil(string img, int id)
        {
            var lista = bd.personas.FirstOrDefault(b => b.id == id);
            lista.fotoperfil = img;
            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void App_upEstatusSolicitud(int idsoli, int estatus)
        {
            var lista = bd.solicituds.FirstOrDefault(a => a.id == idsoli);
            lista.estatus = estatus;
            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void app_GetPerfil(int idusario)
        {
            var query = (from per in bd.personas
                         join u in bd.usuarios on u.idpersona equals u.id
                         where u.id==idusario
                         select new
                         {
                             nombre=per.nombre,
                             apellidos=per.apellido,
                             telefono=per.telefono,
                             correo=u.email,
                             foto=per.fotoperfil

                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }



        // WEB - Administrador --------------------------------------------------------------------------------------------------------------------------      

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddcategoria(string nombre, string actividad1, string actividad2)
        {
            caT.nombre = nombre;
            act.nombre = actividad1;
            actividade act2 = new actividade();
            act2.nombre = actividad2;
            caT.actividades.Add(act);
            caT.actividades.Add(act2);
            bd.categoriaTs.Add(caT);
            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddactividad(string nombre, int idcategorias)
        {
            act.idcategorias = idcategorias;
            act.nombre = nombre;
            bd.actividades.Add(act);
            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }
       
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebuploadIcon(string img, int id)
        {
            string url = img;
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            var lista = bd.categoriaTs.FirstOrDefault(b => b.id == id);
            lista.icono = imageBytes;
            bd.SaveChanges();
        }
         
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUpUserStatus(int iduser, int estatus)
        {            
            var query = from usuario in bd.usuarios
                        where usuario.id == iduser
                        select usuario;
            foreach (usuario usu in query)
            {
                usu.estatus = estatus;
            }
            try
            {
                if (bd.SaveChanges() > 0)
                    json = "1";
                else
                    json = "0";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Se convierte a JSON
            string SalidaJSON = string.Empty;
            SalidaJSON = JsonConvert.SerializeObject(json);
            //Salida del webservice
            HttpContext Contexto = HttpContext.Current;
            Context.Response.ContentType = "application/json";
            Context.Response.Write(SalidaJSON);
            Context.Response.End();
        }       

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddUsuarioAdmin( string email, string pass, string nombre, string apellido, string telefono, string sexo, string curp, string fechanacimiento, string fotoperfil)
        {
            salt = cryptoService.GenerateSalt();
            passEncryptada = cryptoService.Compute(pass);
            {
                p.nombre = nombre;
                p.apellido = apellido;
                p.telefono = telefono;
                p.sexo = sexo;
                p.curp = curp;
                p.fechanacimiento = fechanacimiento;
                p.fotoperfil = fotoperfil;
                p.idinteres = 1;
                u.email = email;
                u.pass = passEncryptada;
                u.salt = salt;
                u.idperfil = 1;
                u.estatus = 1;                
                p.usuarios.Add(u);
                bd.personas.Add(p);
            }
            if (bd.SaveChanges() > 0)
                json = u.id.ToString();
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddPerfil(string perfil)
        {
            perf.tipoperfil = perfil;            
            bd.perfils.Add(perf);            
            if (bd.SaveChanges() > 0)
                json = perf.id.ToString();
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddCategory(string category)
        {
            caT.nombre = category;
            bd.categoriaTs.Add(caT);
            if (bd.SaveChanges() > 0)
                json = caT.id.ToString();
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebAddActividades(int idcate, string actividad)
        {
            act.idcategorias = idcate;
            act.nombre = actividad;
            bd.actividades.Add(act);
            if (bd.SaveChanges() > 0)
                json = "1";
            else
                json = "0";
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetUsers()
        {
            var datos = from p in bd.personas
                        join u in bd.usuarios on p.id equals u.idpersona
                        join per in bd.perfils on u.idperfil equals per.id
                        where per.id == 1
                        select new
                        {
                            ID = u.id,
                            Correo = u.email,
                            Pass = u.pass,
                            Nombres = p.nombre,
                            Apellidos = p.apellido,
                            Telefono = p.telefono,
                            Sexo = p.sexo,
                            Curp = p.curp,
                            Perfil = per.tipoperfil,
                        };
            //Se convierte a JSON
            string SalidaJSON = string.Empty;
            SalidaJSON = JsonConvert.SerializeObject(datos);
            //Salida del webservice
            HttpContext Contexto = HttpContext.Current;
            Context.Response.ContentType = "application/json";
            Context.Response.Write(SalidaJSON);
            Context.Response.End();

        }
        //Inicio Indicador Publicaciones
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllPublic()
        {
            var query = (from p in bd.publicacions
                         select new
                         {
                             total = p.id
                         }).Count();
            json = JsonConvert.SerializeObject(query);
            con.Response.ContentType = "application/json";        
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");            
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllPublicByDate(DateTime inicio, DateTime final)
        {
            var query = (from p in bd.publicacions
                         where p.fecha >= inicio && p.fecha <= final
                         select new
                         {
                             total = p.id
                         }).Count();
            json = JsonConvert.SerializeObject(query);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetPublicCategoryRequests()
        {
            var query = (from ca in bd.categoriaTs
                         join p in bd.publicacions on ca.id equals p.idcategorias
                         join s in bd.solicituds on p.id equals s.idpublicacion
                         group ca by ca.nombre into g
                         select new
                         {                             
                             solicitudes = g.Count(),                             
                         });
            json = JsonConvert.SerializeObject(query);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetPublicCategory()
        {
            var query = (from ca in bd.categoriaTs
                         join p in bd.publicacions on ca.id equals p.idcategorias                         
                         group ca by ca.nombre into g
                         select new
                         {
                             nombre = g.Key,
                             popularidad = g.Count(),
                         });
            json = JsonConvert.SerializeObject(query);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");            
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetPublicCategoryDate(DateTime inicio, DateTime final)
        {
            var query = (from ca in bd.categoriaTs
                         join p in bd.publicacions on ca.id equals p.idcategorias
                         where p.fecha >= inicio && p.fecha <=final
                         group ca by ca.nombre into g
                         select new
                         {
                             nombre = g.Key,
                             popularidad= g.Count(),
                         });
          
            json = JsonConvert.SerializeObject(query);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }       
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllCategory()
        {
            var query = (from ca in bd.categoriaTs
                         join p in bd.publicacions on ca.id equals p.idcategorias
                         group ca by ca.nombre into g
                         select new
                         {
                             total = g.Key
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllCategoryByDate(DateTime inicio, DateTime final)
        {
            var query = (from ca in bd.categoriaTs
                         join p in bd.publicacions on ca.id equals p.idcategorias
                         where p.fecha >= inicio && p.fecha <= final
                         group ca by ca.nombre into g
                         select new
                         {
                             total = g.Key
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        //Fin Indicador Publicaciones
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetSolicitudesPub(DateTime inicio, DateTime final)
        {
            var query = (from s in bd.solicituds
                         join p in bd.publicacions on s.idpublicacion equals p.id
                         where p.fecha >= inicio && p.fecha <= final
                         group s by s.idpublicacion into g
                         select new
                         {
                             idpublicacion = g.Key,
                             solicitudes = g.Count()
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetPerfilUsu()
        {
            var query = (from u in bd.usuarios
                         join per in bd.perfils on u.idperfil equals per.id
                         group per by per.tipoperfil into g
                         select new
                         {
                             tipoperfil = g.Key,
                             popularidad = g.Count()
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllPerfiles()
        {
            var query = (from per in bd.perfils                         
                         group per by per.id into g
                         select new
                         {
                             total = g.Key
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetAllUsers()
        {
            var query = (from u in bd.usuarios
                         group u by u.id into g
                         select new
                         {
                             total = g.Key
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUsuariosReportan()
        {
            var query = (from i in bd.insidencias
                         join u in bd.usuarios on i.idusuario equals u.id
                         join urep in bd.usuarios on i.idusreportado equals urep.id
                         join per in bd.perfils on u.idperfil equals per.id
                         join perrep in bd.perfils on urep.idperfil equals perrep.id
                         group per by per.tipoperfil into g
                         select new
                         {
                             usuarioreportan = g.Key,
                             popularidad = g.Count()
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetUsuariosReportados()
        {
            var query = (from i in bd.insidencias
                         join u in bd.usuarios on i.idusuario equals u.id
                         join urep in bd.usuarios on i.idusreportado equals urep.id
                         join per in bd.perfils on u.idperfil equals per.id
                         join perrep in bd.perfils on urep.idperfil equals perrep.id
                         group perrep by perrep.tipoperfil into g
                         select new
                         {
                             usuarioreportan = g.Key,
                             popularidad = g.Count()
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetCorreoReportan(string correo)
        {
            var query = (from i in bd.insidencias
                         join u in bd.usuarios on i.idusuario equals u.id
                         join urep in bd.usuarios on i.idusreportado equals urep.id
                         where u.email == correo
                         group u by u.id into g
                         select new
                         {                             
                             total = g.Key
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetCorreoReportados(string correo)
        {
            var query = (from i in bd.insidencias
                         join u in bd.usuarios on i.idusuario equals u.id
                         join urep in bd.usuarios on i.idusreportado equals urep.id
                         where urep.email == correo
                         group urep by urep.id into g
                         select new
                         {                          
                             total = g.Key,
                         }).Count();
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetUsuariosSoliTOP()
        {
            var query = (from s in bd.solicituds
                         join u in bd.usuarios on s.idusuario equals u.id                                                  
                         group u by u.email into g
                         select new
                         {
                             usuarios = g.Key,
                             popularidad = g.Count()
                         }).Take(5);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebGetUsuariosPubliTOP()
        {
            var query = (from p in bd.publicacions
                         join u in bd.usuarios on p.idusuario equals u.id
                         group u by u.email into g
                         select new
                         {
                             usuarios = g.Key,
                             popularidad = g.Count()
                         }).Take(5);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }        
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebDeleteUserAdm(int id)
        {
            var remove = (from u in bd.usuarios
                          where u.id == id                     
                          select u).FirstOrDefault();

            if (remove != null)
            {
                bd.usuarios.Remove(remove);
                bd.SaveChanges();
                json = "1";
            }     
            else
                json = "0";            

            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");           
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUpdateAdmin(int id, string email, string pass, string nombres, string apellidos, string telefono, string curp, string sexo)
        {
            //Usuario
            var lista = bd.usuarios.FirstOrDefault(u => u.id == id);
            salt = cryptoService.GenerateSalt();
            passEncryptada = cryptoService.Compute(pass);
            lista.email = email;
            lista.pass = passEncryptada;
            int idpersona = lista.idpersona;
            //persona
            var lista2 = bd.personas.FirstOrDefault(p => p.id == idpersona);
            lista2.nombre = nombres;
            lista2.apellido = apellidos;
            lista2.telefono = telefono;
            lista2.curp = curp;
            lista2.sexo = sexo;

            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        public void WebPerfiles()
        {
            var query = (from p in bd.perfils          
                         select new
                         {
                             id = p.id,
                             perfil = p.tipoperfil,                     
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        public void WebActividades()
        {
            var query = (from a in bd.actividades
                         join c in bd.categoriaTs on a.idcategorias equals c.id
                         select new
                         {
                             id = a.id,
                             categoria = c.nombre,
                             actividad = a.nombre,
                             idcate= c.id
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUpdatePerfil(int id, string perfil)
        {
            var lista = bd.perfils.FirstOrDefault(p => p.id == id);
            lista.tipoperfil = perfil;
            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebDeletePerfil(int id)
        {
            var remove = (from perf in bd.perfils
                          where perf.id == id
                          select perf).FirstOrDefault();

            if (remove != null)
            {
                bd.perfils.Remove(remove);
                bd.SaveChanges();
                json = "1";
            }
            else
                json = "0";

            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUpdateCategor(int id, string category)
        {
            var lista = bd.categoriaTs.FirstOrDefault(cat => cat.id == id);
            lista.nombre = category;
            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebDeleteCategory(int id)
        {
            var remove = (from cat in bd.categoriaTs
                          where cat.id == id
                          select cat).FirstOrDefault();

            if (remove != null)
            {
                bd.categoriaTs.Remove(remove);
                bd.SaveChanges();
                json = "1";
            }
            else
                json = "0";

            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebUpdateActividad(int id, int idcate, string actividades)
        {
            var lista = bd.actividades.FirstOrDefault(a => a.id == id);
            lista.idcategorias = idcate;
            lista.nombre = actividades;
            bd.SaveChanges();
            json = JsonConvert.SerializeObject(1);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WebDeleteActividad(int id)
        {
            var remove = (from a in bd.actividades
                          where a.id == id
                          select a).FirstOrDefault();

            if (remove != null)
            {
                bd.actividades.Remove(remove);
                bd.SaveChanges();
                json = "1";
            }
            else
                json = "0";

            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            con.Response.Write(json);
            con.Response.End();
        }


//No se para que son ggg, salu2 --------------------------------------------------------------------------------------------------------------------------
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetUsersEstatus(int estatus)
        {
            var datos = from p in bd.personas
                        join u in bd.usuarios on p.id equals u.idpersona
                        join per in bd.perfils on u.idperfil equals per.id
                        where u.estatus == estatus && per.id != 1
                        orderby u.id descending
                        select new
                        {
                            ID = u.id,
                            Correo = u.email,
                            Nombres = p.nombre,
                            Apellidos = p.apellido,
                            Telefono = p.telefono,
                            Sexo = p.sexo,
                            Curp = p.curp,
                            Perfil = per.tipoperfil,
                            Imagen = p.fotoperfil,
                        };
            //Se convierte a JSON
            string SalidaJSON = string.Empty;
            SalidaJSON = JsonConvert.SerializeObject(datos);
            //Salida del webservice
            HttpContext Contexto = HttpContext.Current;
            Context.Response.ContentType = "application/json";
            Context.Response.Write(SalidaJSON);
            Context.Response.End();
        }


    }
}

