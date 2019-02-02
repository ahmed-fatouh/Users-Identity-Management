using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersIdentityManagement.Models;
using UsersIdentityManagement.Models.AppUserModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UsersIdentityManagement.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UsersIdentityManagement.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;
        private IUserValidator<AppUser> userValidator;

        public AdminController(UserManager<AppUser> usrMgr, IPasswordValidator<AppUser> pwdValidator,
                               IPasswordHasher<AppUser> pwdHasher, IUserValidator<AppUser> usrValidator)
        {
            userManager = usrMgr;
            passwordValidator = pwdValidator;
            passwordHasher = pwdHasher;
            userValidator = usrValidator;
        }
 

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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return BadRequest();
            AppUser user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            EditModel model = new EditModel(user.Id, user.UserName, user.Email);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();
                user = await AddUserPropertiesToUserAsync(user, model);
                user = await AddPasswordHashToUserAsync(user, model.Password);
                IdentityResult result;
                if (ModelState.ErrorCount == 0)
                {
                    result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    else
                        AddModelStateErrors(result);
                }
            }
            return View(model);
        }

        private void AddModelStateErrors(IdentityResult failedIdentityResult)
        {
            foreach (IdentityError error in failedIdentityResult.Errors)
                ModelState.AddModelError("", error.Description);
        }

        //If the password can't pass validation, the user object is returned with PasswordHash property
        //not set. Also, the ModelState Controller property is populated with validation errors.
        private async Task<AppUser> AddPasswordHashToUserAsync (AppUser user, string password)
        {
            IdentityResult result;
            if (!string.IsNullOrEmpty(password))
            {
                result = await passwordValidator.ValidateAsync(userManager, user, password);
                if (result.Succeeded)
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    //ModelState is available through Controller context, so
                    //you don't need to return it to the calling method.
                    AddModelStateErrors(result);
            }
            return user;
        }

        //The user object is returned with new EditModel Properties except Password even if it's
        //not valid, but the ModelState is populated with validation errors.
        private async Task<AppUser> AddUserPropertiesToUserAsync(AppUser user, EditModel model)
        {
            IdentityResult result;
            user.UserName = model.Name;
            user.Email = model.Email;
            result = await userValidator.ValidateAsync(userManager, user);
            if (!result.Succeeded)
                AddModelStateErrors(result);
            return user;
        }
    }
}
