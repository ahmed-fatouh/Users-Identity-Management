using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UsersIdentityManagement.Models
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class EditModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Display(Name = "Password (No change if left empty.)")]
        public string Password { get; set; }

        public EditModel() { }

        public EditModel(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }

    public class LoginModel
    {
        [Required, UIHint("Email")]
        public string Email { get; set; }

        [Required, UIHint("Password")]
        public string Password { get; set; }
    }

}
