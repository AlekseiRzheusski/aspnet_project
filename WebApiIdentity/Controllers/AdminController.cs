using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApiIdentity.Infrastructure;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using WebApiIdentity.Models;
using Microsoft.AspNet.Identity;

namespace WebApiIdentity.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class AdminController : Controller
    {


        private string error;

        // GET: Admin
        public List<string> classrooms = new List<string> { "101", "102", "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "115", "116", "117", "118", "201", "202", "203", "204", "205", "206", "207", "208", "209", "210", "211", "212", "213", "214", "215", "216", "217", "218", "301", "302", "303", "304", "305", "306", "307", "308", "309", "310", "311", "312", "313", "314", "315", "316", "317", "318", "401", "402", "403", "404", "405", "406", "407", "408", "409", "410", "411", "412", "413", "414", "415", "416", "417", "418" };

        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
        private AppRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppRoleManager>();
            }
        }







        public async Task<ActionResult> Class()
        {
            List<Class> classes = new List<Class>();
            using (var appContext = new TimetableEntities())
            {
                classes = appContext.Class.ToList();
            }

            List<ClassViewModel> classViewModels = new List<ClassViewModel>();

            foreach (var a in classes)
            {
                ClassViewModel tmp = new ClassViewModel();
                tmp.class_id = a.id;
                AppUser user = await UserManager.FindByIdAsync(a.teacher_id);
                if (user != null)
                {
                    tmp.teacher = user;
                }
                classViewModels.Add(tmp);
            }
            return View(classViewModels);

        }


        public ActionResult DeleteClassModalView(string classId)
        {
            StringViewModel str = new StringViewModel();
            str.str = classId;
            
            return PartialView(str);
        }


        public async Task<ActionResult> DeleteClass(string classId)
        {
            using (var appContext = new TimetableEntities())
            {
                List<Schedule> delSchedules = appContext.Schedule.Where(a => a.class_id == classId).ToList();

                foreach(var b in delSchedules)
                {
                    List<Mark> delMarks = appContext.Mark.Where(a => a.schedule_id == b.id).ToList();
                    if (delMarks.Count != 0)
                    {
                        foreach (var tmp in delMarks)
                        {
                            appContext.Entry(tmp).State = System.Data.Entity.EntityState.Deleted;
                        }
                        appContext.SaveChanges();
                    }
                }

                if (delSchedules.Count != 0)
                {
                    foreach (var tmp in delSchedules)
                    {
                        appContext.Entry(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    appContext.SaveChanges();
                }

                var del = appContext.Class.FirstOrDefault(d => d.id == classId);
                if (del != null)
                {
                    appContext.Entry(del).State = System.Data.Entity.EntityState.Deleted;
                    appContext.SaveChanges();
                }
            }
            return RedirectToAction("Class");

        }



        public async Task<ActionResult> EditClass(string id)
        {
            Class tmp_class = new Class();
            using (var appContext = new TimetableEntities())
            {
                tmp_class = appContext.Class.FirstOrDefault(d => d.id == id);
            }

            if (tmp_class != null)
            {
                AppRole role = await RoleManager.FindByNameAsync("Преподаватель");
                string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();

                IEnumerable<AppUser> members
                    = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));

                SelectList teachers;
                teachers = new SelectList(members, "Id", "UserName");
                if (teachers != null)
                {
                    ViewBag.Teachers = teachers;
                    return View(tmp_class);
                }
                else
                {
                    return RedirectToAction("Class");
                }
            }
            else
            {
                return RedirectToAction("Class");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditClass(Class model)
        {
            using (var appContext = new TimetableEntities())
            {
                appContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                appContext.SaveChanges();
            }

            return RedirectToAction("Class");
        }

        public async Task<ActionResult> CreateClass()
        {
            AppRole role = await RoleManager.FindByNameAsync("Преподаватель");
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();

            IEnumerable<AppUser> members
                = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id));

            SelectList teachers;
            teachers = new SelectList(members, "Id", "UserName");
            ViewBag.Teachers = teachers;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateClass(CreateClassModel model)
        {
            Class tmp_class = new Class();
            tmp_class.id = "";
            tmp_class.id += model.grade + model.letter;
            tmp_class.teacher_id = model.teacher;
            using (var appContext = new TimetableEntities())
            {
                appContext.Class.Add(tmp_class);
                await appContext.SaveChangesAsync();
            }

            return RedirectToAction("Class");
        }








        public ActionResult CreateStudent()
        {
            SelectList classes;
            var appContext = new TimetableEntities();


            classes = new SelectList(appContext.Class, "id", "id");

            ViewBag.Classes = classes;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateStudent(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Login, Email = model.Email, name = model.name, surname = model.surname, patronymic = model.patronymic, classId = model.classId };
                IdentityResult result =
                    await UserManager.CreateAsync(user, model.Password);

                if (!UserManager.IsInRole(user.Id, "Ученик"))
                {
                    UserManager.AddToRole(user.Id, "Ученик");
                }
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        public ActionResult CreateTeacher()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeacher(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Login, Email = model.Email, name = model.name, surname = model.surname, patronymic = model.patronymic};
                IdentityResult result =
                    await UserManager.CreateAsync(user, model.Password);

                if (!UserManager.IsInRole(user.Id, "Преподаватель"))
                {
                    UserManager.AddToRole(user.Id, "Преподаватель");
                }
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }





        public ActionResult Create()
        {
            SelectList classes;
            var appContext = new TimetableEntities();


            classes = new SelectList(appContext.Class, "id", "id");

            ViewBag.Classes = classes;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Login, Email = model.Email, name = model.name, surname = model.surname, patronymic = model.patronymic, classId = model.classId };
                IdentityResult result =
                    await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            using (var appContext = new TimetableEntities())
            {
                List<Mark> delMarks = appContext.Mark.Where(a => a.student_id == user.Id).ToList();
                if (delMarks.Count != 0)
                {
                    foreach (var tmp in delMarks)
                    {
                        appContext.Entry(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    appContext.SaveChanges();
                }
            }
                if (user != null)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "Пользователь не найден" });
            }
        }





        public ActionResult Timetable()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> TimetableSearchResult(string id)
        {
            List<Schedule> schedules;
            ScheduleViewModel timetable = new ScheduleViewModel();
            timetable.classId = id;
            using (var appContext = new TimetableEntities())
            {
                if (appContext.Class.FirstOrDefault(a => a.id.Replace(" ", "") == id) != null)
                {

                    schedules = appContext.Schedule.ToList();
                    List<Schedule> searchedTimetable = schedules.Where(a => a.class_id.Replace(" ", "") == timetable.classId).ToList();


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
                    timetable.countDifficulty();
                    return PartialView(timetable);
                }
                else return PartialView("TimetableNotFound");
            }

        }


        public ActionResult Error()
        {
            return View(error);
        }


        public async Task<ActionResult> CreateSchedule(string day_of_week, string classId, TimeSpan time)
        {
            var appContext = new TimetableEntities();
            var searchedTimetable = appContext.Schedule.Where(a => a.class_id == classId).Where(a => a.day_of_week == day_of_week).ToList();
            error = "Невозможно добавить урок. ";

            List<string> availableClassrooms = classrooms;

            AppRole role = await RoleManager.FindByNameAsync("Преподаватель");
            string[] memberIDs = role.Users.Select(x => x.UserId).ToArray();

            List<AppUser> members
                = UserManager.Users.Where(x => memberIDs.Any(y => y == x.Id)).ToList();

            List<Schedule> schedules = appContext.Schedule.Where(a => a.begin_time == time).Where(a => a.day_of_week == day_of_week).ToList();
            if (schedules.Count != 0)
            {

                foreach (var tmp in schedules)
                {
                    members.Remove(members.FirstOrDefault(a => a.Id == tmp.teacher_id));
                    availableClassrooms.Remove(tmp.classroom.Replace(" ",""));
                }
            }
            if (members.Count == 0)
            {
                error += "Нет свободных преподавателей. ";
            }
            if (classrooms.Count == 0)
            {
                error += "Нет свободных аудиторий. ";
            }

            if(members.Count == 0|| classrooms.Count == 0)
            {
                ModelState.AddModelError("", error);
            }
            else
            {
                SelectList teachers;
                teachers = new SelectList(members,"Id","surname");
                ViewBag.Teachers = teachers;
                ViewBag.Classrooms = new SelectList(availableClassrooms);

            }


            SelectList subjects;


            subjects = new SelectList(appContext.Subject, "id", "name");

            ViewBag.Subjects = subjects;


            Schedule schedule = new Schedule();
            schedule.day_of_week = day_of_week;
            schedule.class_id = classId;
            schedule.begin_time = time;
            return View(schedule);

        }

        public ActionResult ViewSchedule(Schedule model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSchedule(Schedule model)
        {
            string classId = model.class_id;
            using (var appContext = new TimetableEntities())
            {
                appContext.Schedule.Add(model);
                await appContext.SaveChangesAsync();
            }

            return RedirectToAction("Timetable");
        }



        public ActionResult DeleteScheduleModalView(int id)
        {
                return PartialView(id);
        }


        public async Task<ActionResult> DeleteSchedule(int scheduleId)
        {
            using (var appContext = new TimetableEntities())
            {
                List<Mark> delMarks = appContext.Mark.Where(a => a.schedule_id == scheduleId).ToList();
                if (delMarks.Count != 0)
                {
                    foreach(var tmp in delMarks)
                    {

                        appContext.Entry(tmp).State = System.Data.Entity.EntityState.Deleted;
                    }
                    appContext.SaveChanges();
                }

                var del = appContext.Schedule.FirstOrDefault(d => d.id == scheduleId);
                if (del != null)
                {
                    appContext.Entry(del).State = System.Data.Entity.EntityState.Deleted;
                    appContext.SaveChanges();
                }
            }
            return RedirectToAction("Timetable");

        }






        public async Task<ActionResult> Edit(string id)
        {
            SelectList classes;
            var appContext = new TimetableEntities();


            classes = new SelectList(appContext.Class, "id", "id");

            ViewBag.Classes = classes;

            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string id, string email, string password)
        {
            AppUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail
                    = await UserManager.UserValidator.ValidateAsync(user);

                if (!validEmail.Succeeded)
                {
                    AddErrorsFromResult(validEmail);
                }

                IdentityResult validPass = null;
                if (password != string.Empty)
                {
                    validPass
                        = await UserManager.PasswordValidator.ValidateAsync(password);

                    if (validPass.Succeeded)
                    {
                        user.PasswordHash =
                            UserManager.PasswordHasher.HashPassword(password);
                    }
                    else
                    {
                        AddErrorsFromResult(validPass);
                    }
                }

                if ((validEmail.Succeeded && validPass == null) ||
                        (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Пользователь не найден");
            }
            return View(user);
        }







        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}