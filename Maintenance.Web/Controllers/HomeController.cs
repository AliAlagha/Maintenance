using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Maintenance.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IUserService userService) : base(userService)
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}