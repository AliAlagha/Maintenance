using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Branches;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BarcodeController : BaseController
    {
        public BarcodeController(IUserService userService) : base(userService)
        {
        }

        public IActionResult Scan(BarcodeSearchType type)
        {
            ViewBag.BarcodeSearchType = type;
            return View();
        }

    }
}

