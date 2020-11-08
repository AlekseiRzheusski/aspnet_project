using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace WebApiIdentity.Models
{
    public class CreateModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string surname { get; set; }

        [Required]
        public string patronymic { get; set; }

        public string classId { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RoleEditModel
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMembers { get; set; }
    }

    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }
        public string[] IdsToAdd { get; set; }
        public string[] IdsToDelete { get; set; }
    }
}