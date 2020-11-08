using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApiIdentity.Models;
using WebApiIdentity.Infrastructure;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;

namespace WebApiIdentity.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {

        [AllowAnonymous]
        public ActionResult Create()
        {
            SelectList classes;
            var appContext = new TimetableEntities();


            classes = new SelectList(appContext.Class, "id", "id");

            ViewBag.Classes = classes;
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser { UserName = model.Login, Email = model.Email, name = model.name, surname = model.surname, patronymic=model.patronymic,classId=model.classId };
                IdentityResult result =
                    await UserManager.CreateAsync(user, model.Password);

                if (!UserManager.IsInRole(user.Id, "Ученик"))
                {
                    UserManager.AddToRole(user.Id, "Ученик");
                }
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }




        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("Error", new string[] { "В доступе отказано" });
            }
            if (returnUrl != null)
            {
                ViewBag.returnUrl = returnUrl;

            }
            else ViewBag.returnUrl = "/";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            AppUser user = await UserManager.FindAsync(details.Name, details.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Некорректное имя или пароль.");
            }
            else
            {
                ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                    DefaultAuthenticationTypes.ApplicationCookie);

                AuthManager.SignOut();
                AuthManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = false
                }, ident);
                return Redirect("/Home/Index");
            }

            return View(details);
        }


        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
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