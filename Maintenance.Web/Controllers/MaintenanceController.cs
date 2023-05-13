using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Maintenance;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    public class MaintenanceController : BaseController
    {
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceController(IUserService userService
            , IMaintenanceService maintenanceService) : base(userService)
        {
            _maintenanceService = maintenanceService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _maintenanceService.GetAllItems(pagination, query, UserId);
            return Json(response);
        }

        public async Task<IActionResult> CompleteMaintenance(int receiptItemId)
        {
            await _maintenanceService.CompleteMaintenance(receiptItemId, UserId);
            return UpdatedSuccessfully();
        }

        public IActionResult CustomerRefuseMaintenance(int receiptItemId)
        {
            var dto = new CustomerRefuseMaintenanceDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CustomerRefuseMaintenance(CustomerRefuseMaintenanceDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.CustomerRefuseMaintenance(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult SuspenseMaintenance(int receiptItemId)
        {
            var dto = new SuspenseReceiptItemDto { ReceiptItemId = receiptItemId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SuspenseMaintenance(SuspenseReceiptItemDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.SuspenseMaintenance(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

        public IActionResult EnterMaintenanceCost(int receiptItemId)
        {
            var dto = new EnterMaintenanceCostDto { ReceiptItemId = receiptItemId };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnterMaintenanceCost(EnterMaintenanceCostDto dto)
        {
            if (ModelState.IsValid)
            {
                await _maintenanceService.EnterMaintenanceCost(dto, UserId);
                return UpdatedSuccessfully();
            }
            return View(dto);
        }

    }
}

