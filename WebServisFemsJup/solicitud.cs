//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebServisFemsJup
{
    using System;
    using System.Collections.Generic;
    
    public partial class solicitud
    {
        public int id { get; set; }
        public int idusuario { get; set; }
        public int idpublicacion { get; set; }
        public int estatus { get; set; }
        public Nullable<System.DateTime> f_citar { get; set; }
        public Nullable<System.DateTime> f_trabajo { get; set; }
        public string descripcion { get; set; }
        public System.DateTime fecha { get; set; }
        public string @long { get; set; }
        public string lat { get; set; }
    
        public virtual publicacion publicacion { get; set; }
        public virtual usuario usuario { get; set; }
    }
}
