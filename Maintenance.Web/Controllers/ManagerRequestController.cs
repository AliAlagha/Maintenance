using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Maintenance;
using Maintenance.Infrastructure.Services.ManagerRequests;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ManagerRequestController : BaseController
    {
        private readonly IManagerRequestService _managerRequestService;

        public ManagerRequestController(IUserService userService
            , IManagerRequestService managerRequestService) : base(userService)
        {
            _managerRequestService = managerRequestService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _managerRequestService.GetAllItems(pagination, query, UserId);
            return Json(response);
        }

        public async Task<IActionResult> UpdateStatus(int receiptItemId, ReturnHandReceiptItemRequestStatus status)
        {
            await _managerRequestService.UpdateStatus(receiptItemId, status, UserId);
            return UpdatedSuccessfully();
        }

    }
}

