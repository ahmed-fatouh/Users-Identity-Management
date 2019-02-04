using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersIdentityManagement.Models;
using Microsoft.AspNetCore.Identity;
using UsersIdentityManagement.Models.AppUserModels;
using Microsoft.AspNetCore.Authorization;

namespace UsersIdentityManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> usrMgr, SignInManager<AppUser> signInMgr)
        {
            userManager = usrMgr;
            signInManager = signInMgr;
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager
                                .PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                        return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Email or Password is incorrect");
            }
            return View(model);
        }
    }
}
