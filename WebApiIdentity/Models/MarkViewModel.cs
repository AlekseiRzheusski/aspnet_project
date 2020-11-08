using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiIdentity.Models
{
    public class MarkViewModel
    {
        public DateTime date { get; set; } 
        public Mark mark { get; set; }
        public AppUser student { get; set; }
        public int scheduleId { get; set; }
        public AppUser teacher { get; set; }
        public string subject { get; set; }
    }
}