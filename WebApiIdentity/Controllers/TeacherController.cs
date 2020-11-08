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
    [Authorize(Roles = "Преподаватель")]
    public class TeacherController : Controller
    {

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        // GET: Teacher
        public async Task<ActionResult> Index()
        {
            string currentUserName = User.Identity.Name;
            AppUser currentUser = await UserManager.FindByNameAsync(currentUserName);
            List<Schedule> searchedTimetable;
            ScheduleViewModel timetable = new ScheduleViewModel();
            using (var appContext = new TimetableEntities())
            {

                searchedTimetable = appContext.Schedule.Where(a => a.teacher_id == currentUser.Id).OrderBy(a => a.begin_time).ToList();


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

        public ActionResult Journal()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> JournalTimetable(string date)
        {
            string searchDayOfTheWeek;
            string currentUserName = User.Identity.Name;
            AppUser currentUser = await UserManager.FindByNameAsync(currentUserName);

            JournalViewModel journal = new JournalViewModel();
            journal.date = DateTime.Parse(date, CultureInfo.CreateSpecificCulture("ru-RU"));
            string dayOfWeek = journal.date.DayOfWeek.ToString();
            switch (dayOfWeek)
            {
                case "Monday": searchDayOfTheWeek = "пн"; break;
                case "Tuesday": searchDayOfTheWeek = "вт"; break;
                case "Wednesday": searchDayOfTheWeek = "ср"; break;
                case "Thursday": searchDayOfTheWeek = "пт"; break;
                case "Friday": searchDayOfTheWeek = "сб"; break;
                default: searchDayOfTheWeek = "выходной"; break;
            }

            if (searchDayOfTheWeek == "выходной")
            {
                return PartialView("ErrorDateInput");
            }

            using (var appContext = new TimetableEntities())
            {
                List<Schedule> searchedTimetable = appContext.Schedule.Where(a => a.teacher_id == currentUser.Id).OrderBy(a => a.begin_time).ToList();
                foreach (var tmp in searchedTimetable)
                {
                    if (tmp.day_of_week.Replace(" ", "") == searchDayOfTheWeek)
                    {
                        LessonViewModel lessonViewModel = new LessonViewModel();
                        lessonViewModel.schedule = tmp;
                        lessonViewModel.subject = appContext.Subject.FirstOrDefault(a => a.id == tmp.subject_id);
                        journal.lessons.Add(lessonViewModel);
                    }
                }
            }

            return PartialView(journal);
        }




        public ActionResult ClassJournal(int scheduleId, string date)
        {
            List<MarkViewModel> classJournal = new List<MarkViewModel>();
            using (var appContext = new TimetableEntities())
            {
                DateTime d = DateTime.Parse(date, CultureInfo.CreateSpecificCulture("en-US"));
                Schedule schedule = appContext.Schedule.FirstOrDefault(a => a.id == scheduleId);
                List<AppUser> students = UserManager.Users.Where(a => a.classId == schedule.class_id).ToList();
                foreach (var a in students)
                {
                    MarkViewModel studentMark = new MarkViewModel();
                    studentMark.student = a;
                    studentMark.mark = appContext.Mark.Where(tmp => tmp.date == d).Where(tmp => tmp.student_id == a.Id).FirstOrDefault(tmp=>tmp.schedule_id==scheduleId);
                    studentMark.date = d;
                    studentMark.scheduleId = scheduleId;
                    classJournal.Add(studentMark);
                }
            }
            return View(classJournal);
        }

        [HttpPost]
        public async Task<ActionResult> AddMark(string studentId, string mark, DateTime date, int scheduleId)
        {
            Mark mark1 = new Mark();
            mark1.mark1 = mark;
            mark1.student_id = studentId;
            mark1.date = date;
            mark1.schedule_id = scheduleId;
            using (var appContext = new TimetableEntities())
            {
                appContext.Mark.Add(mark1);
                await appContext.SaveChangesAsync();
            }

            return RedirectToAction("ClassJournal",new {scheduleId=scheduleId,date=date.ToString(new CultureInfo("en-US")) });
        }

        public async Task<ActionResult> AddAbsence(string studentId, DateTime date, int scheduleId)
        {
            Mark mark1 = new Mark();
            mark1.mark1 = "н";
            mark1.student_id = studentId;
            mark1.date = date;
            mark1.schedule_id = scheduleId;
            using (var appContext = new TimetableEntities())
            {
                appContext.Mark.Add(mark1);
                await appContext.SaveChangesAsync();
            }

            return RedirectToAction("ClassJournal", new { scheduleId = scheduleId, date = date.ToString(new CultureInfo("en-US")) });
        }

        public async Task<ActionResult> DeleteMark(int markId, DateTime date, int scheduleId)
        {

            using (var appContext = new TimetableEntities())
            {
                var del = appContext.Mark.FirstOrDefault(d => d.id == markId);
                if (del != null)
                {
                    appContext.Entry(del).State = System.Data.Entity.EntityState.Deleted;
                    appContext.SaveChanges();
                }
            }

            return RedirectToAction("ClassJournal", new { scheduleId = scheduleId, date = date.ToString(new CultureInfo("en-US")) });
        }


    }
}