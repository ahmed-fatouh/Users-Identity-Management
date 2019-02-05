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

    }
}
