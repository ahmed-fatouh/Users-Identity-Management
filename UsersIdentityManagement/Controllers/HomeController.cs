using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UsersIdentityManagement.Controllers
{

    [Authorize]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index() => View(GetData(nameof(Index)));

        [Authorize(Roles = "Admins")]
        public IActionResult OtherAction() => View("Index", GetData(nameof(OtherAction)));

        private Dictionary<string, object> GetData(string actionName)
        {
            Dictionary<string, object> details = new Dictionary<string, object>
            {
                ["Action"] = actionName,
                ["User"] = HttpContext.User.Identity.Name,
                ["Is Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
                ["Authentication Type"] = HttpContext.User.Identity.AuthenticationType,
                ["Is In Admins Role"] = HttpContext.User.IsInRole("Admins"),
            }; 
            return details;
        }
    }
}
