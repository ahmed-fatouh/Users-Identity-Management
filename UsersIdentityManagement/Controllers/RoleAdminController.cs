using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UsersIdentityManagement.Models.AppUserModels;
using UsersIdentityManagement.Models;

namespace UsersIdentityManagement.Controllers
{
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<AppUser> userManager;

        public RoleAdminController(RoleManager<IdentityRole> roleMgr, UserManager<AppUser> usrMgr)
        {
            roleManager = roleMgr;
            userManager = usrMgr;
        }

        public ViewResult Index() => View(roleManager.Roles);

        [HttpGet]
        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));
                else
                    AddModelStateErrors(result);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IActionResult actionResult = await CheckIdAndRoleAsync(id);
            if (actionResult != null)
                return actionResult;
            IdentityRole role = await roleManager.FindByIdAsync(id);
            IdentityResult result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));
            else
            {
                string errors = "Can't Delete Role: \n";
                result.Errors.ToList().ForEach(e => errors += e + "\n");
                return Content(errors);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            IActionResult actionResult = await CheckIdAndRoleAsync(id);
            if (actionResult != null)
                return actionResult;
            IdentityRole role = await roleManager.FindByIdAsync(id);
            RoleEditModel model = new RoleEditModel() {
                Role = role,
                Members = new List<AppUser>(),
                NonMembers = new List<AppUser>()
            };
            
            foreach (AppUser user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                    model.Members = model.Members.Append(user);
                else
                    model.NonMembers = model.NonMembers.Append(user);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            if (ModelState.IsValid)
            {
                IActionResult actionResult = await CheckIdAndRoleAsync(model.RoleId);
                if (actionResult != null)
                    return actionResult;
                IdentityRole role = await roleManager.FindByIdAsync(model.RoleId);
                if (role.Name != model.RoleName)
                    return BadRequest();

                IdentityResult result;
                foreach (string id in model.IdsToAdd ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        result = await userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            AddModelStateErrors(result);
                    }
                    else
                        ModelState.AddModelError("", $"User '{user.UserName}' not found");
                }

                foreach (string id in model.IdsToDelete ?? new string[] { })
                {
                    AppUser user = await userManager.FindByIdAsync(id);
                    if (user != null)
                    {
                        result = await userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            AddModelStateErrors(result);
                    }
                    else
                        ModelState.AddModelError("", $"User '{user.UserName}' not found");
                }

                if (ModelState.ErrorCount == 0)
                    return RedirectToAction(nameof(Index));
            }
            return await Edit(model.RoleId);
        }

        #region GenericHelperMethods
        private void AddModelStateErrors(IdentityResult failedIdentityResult)
        {
            foreach (IdentityError error in failedIdentityResult.Errors)
                ModelState.AddModelError("", error.Description);
        }

        //Checks if a non-null Id value is sent and if a role with this id is found.
        //Otherwise, it returns null.
        private async Task<IActionResult> CheckIdAndRoleAsync(string id)
        {
            IdentityRole role = null;
            if (id == null)
                return BadRequest();
            role = await roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();
            return null;
        }
        #endregion
    }
}
