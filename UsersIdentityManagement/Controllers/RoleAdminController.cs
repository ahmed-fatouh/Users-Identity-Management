using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UsersIdentityManagement.Controllers
{
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> roleManager;

        public RoleAdminController(RoleManager<IdentityRole> roleMgr)
        {
            roleManager = roleMgr;
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
