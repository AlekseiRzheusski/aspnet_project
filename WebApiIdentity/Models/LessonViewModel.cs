using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiIdentity.Models
{
    public class LessonViewModel
    {
       public  Schedule schedule { get; set; }
       public AppUser teacher { get; set; }
       public Subject subject { get; set; }

    }
}