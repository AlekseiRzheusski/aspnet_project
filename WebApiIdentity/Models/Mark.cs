//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApiIdentity.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Mark
    {
        public int id { get; set; }
        public string student_id { get; set; }
        public int schedule_id { get; set; }
        public string mark1 { get; set; }
        public System.DateTime date { get; set; }
    
        public virtual Schedule Schedule { get; set; }
    }
}