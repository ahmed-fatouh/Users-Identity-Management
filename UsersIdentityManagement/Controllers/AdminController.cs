using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersIdentityManagement.Models;
using UsersIdentityManagement.Models.AppUserModels;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UsersIdentityManagement.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;

        public AdminController(UserManager<AppUser> usrMgr)
            => userManager = usrMgr;

        public IActionResult Index() => View(userManager.Users);

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

    }
}
