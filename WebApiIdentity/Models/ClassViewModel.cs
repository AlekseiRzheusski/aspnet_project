using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiIdentity.Models
{
    public class ClassViewModel
    {
        public string class_id { get; set; }
        public AppUser teacher { get; set; }
    }

    public class CreateClassModel 
    {
        public string grade { get; set; }
        public string letter { get; set; }

        public string teacher { get; set; }
    }
}