using Maintenance.Core.Enums;
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
            if (CurrentUserType == UserType.Administrator)
            {
                return Redirect("/User/Index");
            }
            else if (CurrentUserType == UserType.MaintenanceManager)
            {
                return Redirect("/HandReceipt/Index");
            }
            else if (CurrentUserType == UserType.MaintenanceTechnician)
            {
                return Redirect("/Maintenance/HandReceiptItems");
            }

            return null;
        }
    }
}