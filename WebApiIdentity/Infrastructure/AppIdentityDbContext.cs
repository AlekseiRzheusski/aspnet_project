using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebApiIdentity.Infrastructure;
using Microsoft.AspNet.Identity;

namespace WebApiIdentity.Models
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext() : base("name=IdentityDb") { }

        static AppIdentityDbContext()
        {
            Database.SetInitializer<AppIdentityDbContext>(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }
        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));

            string AdminRoleName = "Администратор";
            string TeacherRoleName = "Преподаватель";
            string StudentRoleName = "Ученик";
            string ParentRoleName = "Родитель";

            string userName = "Admin";
            string password = "adminpass";
            string email = "admin@admin.ru";

            if (!roleMgr.RoleExists(AdminRoleName))
            {
                roleMgr.Create(new AppRole(AdminRoleName));
            }

            if (!roleMgr.RoleExists(TeacherRoleName))
            {
                roleMgr.Create(new AppRole(TeacherRoleName));
            }

            if (!roleMgr.RoleExists(StudentRoleName))
            {
                roleMgr.Create(new AppRole(StudentRoleName));
            }

            if (!roleMgr.RoleExists(ParentRoleName))
            {
                roleMgr.Create(new AppRole(ParentRoleName));
            }

            AppUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userName, Email = email },
                    password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, AdminRoleName))
            {
                userMgr.AddToRole(user.Id, AdminRoleName);
            }
        }
    }
}