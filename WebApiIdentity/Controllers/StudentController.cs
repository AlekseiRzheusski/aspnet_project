using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApiIdentity.Infrastructure;
using WebApiIdentity.Models;

namespace WebApiIdentity.Controllers
{
    [Authorize(Roles = "Ученик")]
    public class StudentController : Controller
    {

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }


        public ActionResult DiarySearch()
        {
            return View();
        }


        [HttpPost]
        public async Task<PartialViewResult> DiarySearchResult(string firstDate,string secondDate)
        {
            string currentUserName = User.Identity.Name;
            AppUser currentUser = await UserManager.FindByNameAsync(currentUserName);
            DateTime d1 = DateTime.Parse(firstDate, CultureInfo.CreateSpecificCulture("en-US"));
            DateTime d2 = DateTime.Parse(secondDate, CultureInfo.CreateSpecificCulture("en-US"));
            if (d1 > d2)
            {
                return PartialView("ErrorDateInput");
            }

            else
            {
                List<Mark> marks;
                DiaryViewModel diary;
                using (var appContext = new TimetableEntities())
                {
                    diary = new DiaryViewModel();
                    marks = appContext.Mark.Where(a => a.date >= d1 && a.date <= d2).Where(a=>a.student_id==currentUser.Id).ToList();
                    if (marks.Count == 0)
                    {
                        return PartialView("EmptyDate");
                    }

                    else
                    {
                        foreach(var tmp in marks)
                        {
                            MarkViewModel mark = new MarkViewModel();
                            mark.mark = tmp;
                            Schedule schedule = appContext.Schedule.FirstOrDefault(a => a.id == mark.scheduleId);
                            mark.teacher= await UserManager.FindByIdAsync(schedule.teacher_id);
                            mark.date = tmp.date;
                            Subject sbj = appContext.Subject.FirstOrDefault(a => a.id == schedule.subject_id);
                            mark.subject = sbj.name;
                            diary.diary.Add(mark);
                        }
                    return PartialView(diary);
                    }

                }
            }
        }


        // GET: Student
        public async Task<ActionResult> Index()
        {
            string currentUserName = User.Identity.Name;
            AppUser currentUser = await UserManager.FindByNameAsync(currentUserName);

            List<Schedule> searchedTimetable;
            ScheduleViewModel timetable = new ScheduleViewModel();
            timetable.classId = currentUser.classId;
            using (var appContext = new TimetableEntities())
            {

                searchedTimetable = appContext.Schedule.Where(a => a.class_id == currentUser.classId).OrderBy(a => a.begin_time).ToList();


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
                return View(timetable);
                
            }
        }




    }
}