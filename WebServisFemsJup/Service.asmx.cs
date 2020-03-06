using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Script.Services;
using SimpleCrypto;
using System.Data.Entity;
using System.Data;
using System.Net;
using System.Data.Entity.SqlServer;

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
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void Login(string email, string pass,int perfil)
        {
            var list = bd.usuarios.Where(x => x.email == email && x.idperfil == perfil).FirstOrDefault();
            if (list != null)
            {
                passEncryptada = cryptoService.Compute(pass, list.salt);
                if (cryptoService.Compare(list.pass,passEncryptada))
                {
                    var lista = (from us in bd.usuarios
                                 join pe in bd.personas on us.idpersona equals pe.id
                                 where us.id == list.id
                                  select new
                                  {
                                      id = us.id,
                                      idperfil = us.idperfil,
                                      nombre =pe.nombre,
                                      apellido=pe.apellido,
                                      telefono=pe.telefono,
                                      sexo=pe.sexo,
                                      curp=pe.curp,
                                      fechanacimiento=pe.fechanacimiento,
                                      longi=pe.@long,
                                      lat=pe.lat,
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
        public void GetPersona(int id)
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
        public void GetUsersEstatus(int estatus)
        {
            var datos = from p in bd.personas
                        join u in bd.usuarios on p.id equals u.idpersona
                        join per in bd.perfils on u.idperfil equals per.id
                        where u.estatus == estatus && per.id != 1
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
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GeTPublicacion(string id)
        {
            var list = (from pub in bd.publicacions 
                        join c in bd.categoriaTs on pub.idcategorias equals c.id
                        join us in bd.usuarios on pub.idusuario equals us.id
                        join pe in bd.personas on us.idpersona equals pe.id
                        where c.id.ToString().Contains(id) && pub.estatus == 1 //aprobado
                        select new
                        {
                            Titulo = pub.titulo,
                            descripcion = pub.descripcion,
                            fecha = pub.fecha,
                            tarifa = pub.tarifa,
                            extra = pub.extra,
                            dispo = pub.dispo,
                            lat = pub.lat,
                            longi =pub.@long,
                            empleada=pe.nombre+" "+pe.apellido,
                            actividad = bd.actividades.Join(bd.actividadesPublicacions, a=>a.id, z=>z.idactivid,(a,z)=>new { a =a, z=z }).Where(x => x.z.idpublicacion == pub.id).Select(ac => new { 
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
        public void GeTPublicacionPorUser(int id)
        {
            var list = (from pub in bd.publicacions
                        join c in bd.categoriaTs on pub.idcategorias equals c.id
                        join us in bd.usuarios on pub.idusuario equals us.id
                        join pe in bd.personas on us.idpersona equals pe.id
                        where pub.idusuario == id
                        select new
                        {
                            idpublicacion = pub.id,
                            idusuario= pub.idusuario,
                            Titulo = pub.titulo,
                            descripcion = pub.descripcion,
                            fecha = pub.fecha,
                            tarifa = pub.tarifa,
                            extra = pub.extra,
                            dispo = pub.dispo,
                            lat = pub.lat,
                            longi = pub.@long,
                            empleada = pe.nombre + " " + pe.apellido,
                            //actividad = bd.actividades.Join(bd.actividadesPublicacions, a => a.id, z => z.idactivid, (a, z) => new { a = a, z = z }).Where(x => x.z.idpublicacion == pub.id).Select(ac => new {
                            //    ac.a.nombre
                            //}),
                            interes = c.id,
                            //icono = c.icono,
                            radio = pub.radio

                        });
            json = JsonConvert.SerializeObject(list);
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
                            id=cat.id,
                            nombre=cat.nombre,
                            icono = cat.icono
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
        
        //ALTAS
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddUsuario(
            string email,
            string pass, 
            int idperfil,
            string nombre, 
            string apellido, 
            string telefono, 
            string sexo, 
            string curp, 
            string fechanacimiento, 
            string fotoperfil, 
            string longi, 
            string lat, 
            int idinteres
            )
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
        public void Addcalificacion(int idusuario, int puntuacion, string comentario)
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
        public void Addcategoria(string nombre, string actividad1, string actividad2)
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
        public void Addactividad(string nombre, int idcategorias)
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
        public void Addinsidencia(string mensaje, int idusuario, int idusreportado, int estatus, int tipo, string fecha)
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
        public void Addpublicacion(int idcategorias, int idusuario, string descripcion, string fecha,int tarifa, string extra, int estatus, string dispo, string longi, string lat, string actividades, string titulo)
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
            bd.publicacions.Add(pub);
            int res = bd.SaveChanges();
            string[] actividad = actividades.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < actividad.Length; i++) {
                a_p.idactivid = Convert.ToInt32(actividad[i]);
                a_p.idpublicacion = pub.id;
                bd.actividadesPublicacions.Add(a_p);
                bd.SaveChanges();
            }
            if (res > 0)
                json = "1";
            else
                json = "0";

            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void AddSolicitud(int idusuario, int idpublicacion, string f_citar, string f_trabajo, string descripcion, int estatus, string @long, string lat)
        {
            sol.idusuario = idusuario;
            sol.idpublicacion = idpublicacion;
            sol.estatus = estatus;
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
        
        //Cambios
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void upPersona(string email, string pass, string nombre, string apellido, string telefono, string sexo, string curo, string fechanacimiento, string fotoperfil, string @long, string lat, int idinteres, string documento)
        {

        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void uploadIcon(string img, int id)
        {
            string url = img;
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(url);
            var lista = bd.categoriaTs.FirstOrDefault(b => b.id == id);
            lista.icono = imageBytes;
            bd.SaveChanges();
        }
        
        //Administrador    
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
        public void WebAddUsuarioAdmin(
            string email,
            string pass,
            string nombre,
            string apellido,
            string telefono,
            string sexo,
            string curp,
            string fechanacimiento,
            string fotoperfil
            )
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
                         }).Take(3);
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
                         }).Take(3);
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetSolicitudes(int idpublic)
        {
            var query = (from s in bd.solicituds
                         join p in bd.publicacions on s.idpublicacion equals p.id
                         join u in bd.usuarios on s.idusuario equals u.id
                         join per in bd.personas on u.idpersona equals per.id
                         where p.id== idpublic
                         select new
                         {
                             titulo = p.titulo,
                             nombre = per.nombre,
                             apellido = per.apellido
                         });
            con.Response.ContentType = "application/json";
            con.Response.AddHeader("Access-Control-Allow-Origin", "*");
            json = JsonConvert.SerializeObject(query);
            con.Response.Write(json);
            con.Response.End();
        }
        public void UploadFile()//Subir imagen desde android al servidor
        {
            var request = HttpContext.Current.Request;
            if (request != null)
            {
                var photo = request.Files["file"];
                if (photo != null)
                {
                    json = JsonConvert.SerializeObject(photo.FileName);
                    photo.SaveAs(HttpContext.Current.Server.MapPath("img/" + photo.FileName));
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
    }
}
