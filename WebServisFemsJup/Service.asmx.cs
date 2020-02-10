﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Script.Services;
using SimpleCrypto;

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
        public void GeTPublicacion(string id)
        {
            var list = (from pub in bd.publicacions 
                        join c in bd.categoriaTs on pub.idcategorias equals c.id
                        join us in bd.usuarios on pub.idusuario equals us.id
                        join pe in bd.personas on us.idpersona equals pe.id
                        where c.id.ToString().Contains(id) && pub.estatus == 1 //aprobado
                        select new
                        {
                            Titulo = c.nombre,
                            descripcion = pub.descripcion,
                            fecha = pub.fecha,
                            tarifa = pub.tarifa,
                            extra = pub.extra,
                            dispo = pub.dispo,
                            longi=pub.@long,
                            lat=pub.lat,
                            empleada=pe.nombre+" "+pe.apellido,
                            actividad = bd.actividades.Join(bd.actividadesPublicacions, a=>a.id, z=>z.idactivid,(a,z)=>new { a =a, z=z }).Where(x => x.z.idpublicacion == pub.id).Select(ac => new { 
                                 ac.a.nombre
                            })

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
                
                p.usuarios.Add(u);
                bd.personas.Add(p);
            }
            if (bd.SaveChanges() > 0)
                json = "1";
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
        public void Addpublicacion(int idcategorias, int idusuario, string descripcion, string fecha,int tarifa, string extra, int estatus, string dispo, string longi, string lat, string actividades)
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
    }
}
