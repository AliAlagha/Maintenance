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
    [Authorize(Roles = "MaintenanceTechnician")]
    public class MaintenanceController : BaseController
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IUserService userService
            , IMaintenanceService maintenanceService) : base(userService)
        {
            _maintenanceService = maintenanceService;
        }

        public IActionResult HandReceiptItems()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllHandReceiptItems(Pagination pagination, QueryDto query)
        {
            var response = await _maintenanceService.GetAllHandReceiptItems(pagination, query, UserId);
            return Json(response);
        }

        public IActionResult ReturnHandReceiptItems()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllReturnHandReceiptItems(Pagination pagination, QueryDto query)
        {
            var response = await _maintenanceService.GetAllReturnHandReceiptItems(pagination, query, UserId);
            return Json(response);
        }

        // Hand receipt items
        public async Task<IActionResult> UpdateStatusForHandReceipt(int receiptItemId)
        {
            await _maintenanceService.UpdateStatusForHandReceipt(receiptItemId, UserId);
            return UpdatedSuccessfully();
        }

        public IActionResult CustomerRefuseMaintenanceForHandReceipt(int receiptItemId)
        {
            var dto = new CustomerRefuseMaintenanceDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRefuseMaintenanceForHandReceipt(CustomerRefuseMaintenanceDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.CustomerRefuseMaintenanceForHandReceipt(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult SuspenseMaintenanceForHandReceipt(int receiptItemId)
        {
            var dto = new SuspenseReceiptItemDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SuspenseMaintenanceForHandReceipt(SuspenseReceiptItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.SuspenseMaintenanceForHandReceipt(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult EnterMaintenanceCostForHandReceipt(int receiptItemId)
        {
            var dto = new EnterMaintenanceCostDto { ReceiptItemId = receiptItemId };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnterMaintenanceCostForHandReceipt(EnterMaintenanceCostDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.EnterMaintenanceCostForHandReceipt(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        // Return hand receipt items
        public async Task<IActionResult> UpdateStatusForReturnHandReceipt(int receiptItemId)
        {
            await _maintenanceService.UpdateStatusForReturnHandReceipt(receiptItemId, UserId);
            return UpdatedSuccessfully();
        }

        public IActionResult SuspenseMaintenanceForReturnHandReceipt(int receiptItemId)
        {
            var dto = new SuspenseReceiptItemDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SuspenseMaintenanceForReturnHandReceipt(SuspenseReceiptItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.SuspenseMaintenanceForReturnHandReceipt(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }
    }
}

