using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Maintenance;
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
        public async Task<IActionResult> UpdateStatusForHandReceiptItem(int receiptItemId, HandReceiptItemRequestStatus? status)
        {
            await _maintenanceService.UpdateStatusForHandReceiptItem(receiptItemId, status, UserId);
            return UpdatedSuccessfully();
        }

        public IActionResult CustomerRefuseMaintenanceForHandReceiptItem(int receiptItemId)
        {
            var dto = new CustomerRefuseMaintenanceDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRefuseMaintenanceForHandReceiptItem(CustomerRefuseMaintenanceDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.CustomerRefuseMaintenanceForHandReceiptItem(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult SuspenseMaintenanceForHandReceiptItem(int receiptItemId)
        {
            var dto = new SuspenseReceiptItemDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SuspenseMaintenanceForHandReceiptItem(SuspenseReceiptItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.SuspenseMaintenanceForHandReceiptItem(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult EnterMaintenanceCostForHandReceiptItem(int receiptItemId)
        {
            var dto = new EnterMaintenanceCostDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> EnterMaintenanceCostForHandReceiptItem(EnterMaintenanceCostDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.EnterMaintenanceCostForHandReceiptItem(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult DefineMalfunctionForHandReceiptItem(int receiptItemId)
        {
            var dto = new DefineMalfunctionDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> DefineMalfunctionForHandReceiptItem(DefineMalfunctionDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.DefineMalfunctionForHandReceiptItem(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        // Return hand receipt items
        public async Task<IActionResult> UpdateStatusForReturnHandReceiptItem(int receiptItemId)
        {
            await _maintenanceService.UpdateStatusForReturnHandReceiptItem(receiptItemId, UserId);
            return UpdatedSuccessfully();
        }

        public IActionResult SuspenseMaintenanceForReturnHandReceiptItem(int receiptItemId)
        {
            var dto = new SuspenseReceiptItemDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SuspenseMaintenanceForReturnHandReceiptItem(SuspenseReceiptItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.SuspenseMaintenanceForReturnHandReceiptItem(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }
    }
}

