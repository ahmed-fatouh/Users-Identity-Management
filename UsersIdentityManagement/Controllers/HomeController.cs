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

        public IActionResult Index()
        {
            Dictionary<string, object> details;
            details = new Dictionary<string, object> { ["Placeholder"] = "PlaceHolder" };
            return View(details);
        }
    }
}
