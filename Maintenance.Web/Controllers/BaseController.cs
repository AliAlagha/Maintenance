using Maintenance.Core.Constants;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Maintenance.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly IUserService _userService;
        protected string UserId;
        protected UserType CurrentUserType;
        protected bool IsArabic;

        public BaseController(IUserService userService)
        {
            _userService = userService;
        }

        [NonAction]
        protected IActionResult CreatedSuccessfully(string message = null, string link = null)
        {
            return Ok(ResponseConst.GetSuccessResponse(string.IsNullOrEmpty(message) ? Messages.ItemCreatedSuccessfully : message, link));
        }

        [NonAction]
        protected IActionResult UpdatedSuccessfully(string message = null, string link = null)
        {
            return Ok(ResponseConst.GetSuccessResponse(string.IsNullOrEmpty(message) ? Messages.ItemUpdatedSuccessfully : message, link));
        }

        [NonAction]
        protected IActionResult DeletedSuccessfully(string message = null, string link = null)
        {
            return Ok(ResponseConst.GetSuccessResponse(string.IsNullOrEmpty(message) ? Messages.ItemDeletedSuccessfully : message, link));
        }

        [NonAction]
        protected IActionResult ActivatedSuccessfully(string message = null, string link = null)
        {
            return Ok(ResponseConst.GetSuccessResponse(string.IsNullOrEmpty(message) ? Messages.ItemActivatedSuccessfully : message, link));
        }

        [NonAction]
        protected IActionResult DeActivatedSuccessfully(string message = null, string link = null)
        {
            return Ok(ResponseConst.GetSuccessResponse(string.IsNullOrEmpty(message) ? Messages.ItemDeActivatedSuccessfully : message, link));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            IsArabic = System.Globalization.CultureInfo.CurrentCulture.Name == "ar-EG";
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var user = _userService.GetUserByUsername(userName);
                UserId = user.Id;
                CurrentUserType = user.UserType;

                ViewBag.UserId = user.Id;
                ViewBag.FullName = user.FullName;
                ViewBag.FullNameFirstChar = user.FullName.Substring(0, 1);
                ViewBag.UserEmail = user.Email;
                ViewBag.UserType = user.UserType.ToString();
                ViewBag.UserImageFilePath = user.ImageFilePath;
            }
        }

        [NonAction]
        protected ActionResult GetExcelFileResult(byte[] content, string fileName)
        {
            return File(
               content,
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
               $"{fileName} - {DateTime.Now:yyyy_MM_dd}.xlsx"
           );
        }

        [NonAction]
        protected IActionResult GetPdfFileResult(byte[] report, string fileName)
        {
            return File(report, "application/pdf", $"{fileName} - {DateTime.Now:yyyy_MM_dd}.pdf");
        }

    }
}
