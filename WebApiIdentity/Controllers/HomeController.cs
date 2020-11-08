using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApiIdentity.Infrastructure;
using WebApiIdentity.Models;

namespace WebApiIdentity.Controllers
{
    public class HomeController : Controller
    {
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
        [Authorize]
        public ActionResult Index()
        {
            if (HttpContext.User.IsInRole("Администратор"))
            {
                return Redirect("/Admin/Index");
            }

            if (HttpContext.User.IsInRole("Ученик"))
            {
                return Redirect("/Student/Index");
            }

            if (HttpContext.User.IsInRole("Преподаватель"))
            {
                return Redirect("/Teacher/Index");
            }


            else
            {
                return View(GetData("Index"));

            }
        }



        public ActionResult Main()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> MainSearch(string id)
        {
            List<Schedule> searchedTimetable;
            ScheduleViewModel timetable = new ScheduleViewModel();
            using (var appContext = new TimetableEntities())
            {
                if (appContext.Class.FirstOrDefault(a => a.id.Replace(" ", "") == id) != null)
                {
                    searchedTimetable = appContext.Schedule.Where(a => a.class_id == id).OrderBy(a => a.begin_time).ToList();


                    foreach (var tmp in searchedTimetable)
                    {
                        if (tmp.day_of_week.Replace(" ", "") == "пн")
                        {
                            LessonViewModel lessonViewModel = new LessonViewModel();
                            lessonViewModel.schedule = tmp;
                            lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                            lessonViewModel.teacher = await UserManager.FindByIdAsync(tmp.teacher_id);
                            timetable.mondayLessons.Add(lessonViewModel);
                        }
                        if (tmp.day_of_week.Replace(" ", "") == "вт")
                        {
                            LessonViewModel lessonViewModel = new LessonViewModel();
                            lessonViewModel.schedule = tmp;
                            lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                            lessonViewModel.teacher = await UserManager.FindByIdAsync(tmp.teacher_id);
                            timetable.tuesdayLessons.Add(lessonViewModel);
                        }
                        if (tmp.day_of_week.Replace(" ", "") == "ср")
                        {
                            LessonViewModel lessonViewModel = new LessonViewModel();
                            lessonViewModel.schedule = tmp;
                            lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                            lessonViewModel.teacher = await UserManager.FindByIdAsync(tmp.teacher_id);
                            timetable.wednesdayLessons.Add(lessonViewModel);
                        }
                        if (tmp.day_of_week.Replace(" ", "") == "чт")
                        {
                            LessonViewModel lessonViewModel = new LessonViewModel();
                            lessonViewModel.schedule = tmp;
                            lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                            lessonViewModel.teacher = await UserManager.FindByIdAsync(tmp.teacher_id);
                            timetable.thursdayLessons.Add(lessonViewModel);
                        }
                        if (tmp.day_of_week.Replace(" ", "") == "пт")
                        {
                            LessonViewModel lessonViewModel = new LessonViewModel();
                            lessonViewModel.schedule = tmp;
                            lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                            lessonViewModel.teacher = await UserManager.FindByIdAsync(tmp.teacher_id);
                            timetable.fridayLessons.Add(lessonViewModel);
                        }
                    }
                    return PartialView(timetable);
                }
                else return PartialView("TimetableNotFound");
            }


        }


        public PartialViewResult TestPartial(string id)
        {
            return PartialView(id);
        }





        [Authorize]
        public ActionResult OtherAction()
        {
            return View("Index", GetData("OtherAction"));
        }

        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("Action", actionName);
            dict.Add("Пользователь", HttpContext.User.Identity.Name);
            dict.Add("Аутентифицирован?", HttpContext.User.Identity.IsAuthenticated);
            dict.Add("Тип аутентификации", HttpContext.User.Identity.AuthenticationType);
            dict.Add("В роли Users?", HttpContext.User.IsInRole("Ученик"));

            return dict;
        }

       
    }
}