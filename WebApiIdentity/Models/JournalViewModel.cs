using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiIdentity.Models
{
    public class JournalViewModel
    {
        public DateTime date { get; set; }
        public List<LessonViewModel> lessons = new List<LessonViewModel>();
    }
}