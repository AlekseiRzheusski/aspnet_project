using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApiIdentity.Models
{


    public class AppUser : IdentityUser
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string patronymic { get; set; }
        public string classId { get; set; }

        public override string ToString()
        {
            return surname+" "+name+" "+patronymic;
        }
    }
}