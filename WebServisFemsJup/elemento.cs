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
    
    public partial class elemento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public elemento()
        {
            this.elementopermisoes = new HashSet<elementopermiso>();
        }
    
        public int id { get; set; }
        public Nullable<int> idmodulo { get; set; }
        public string tipoelemento { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<elementopermiso> elementopermisoes { get; set; }
        public virtual modulo modulo { get; set; }
    }
}
