﻿using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IUserService userService
            , IReportService handReceiptService) : base(userService)
        {
            _reportService = handReceiptService;
        }

        public IActionResult ReceiptItems()
        {
            return View();
        }

        public async Task<JsonResult> ReceiptItemsReport(QueryDto query)
        {
            var result = await _reportService.ReceiptItemsReport(query);
            return Json(result);
        }

        public IActionResult DeliveredItems()
        {
            return View();
        }

        public async Task<JsonResult> DeliveredItemsReport(QueryDto query)
        {
            var result = await _reportService.DeliveredItemsReport(query);
            return Json(result);
        }

        public IActionResult ReturnedItems()
        {
            return View();
        }

        public async Task<JsonResult> ReturnedItemsReport(QueryDto query)
        {
            var result = await _reportService.ReturnedItemsReport(query);
            return Json(result);
        }

        public IActionResult UrgentItems()
        {
            return View();
        }

        public async Task<JsonResult> UrgentItemsReport(QueryDto query)
        {
            var result = await _reportService.UrgentItemsReport(query);
            return Json(result);
        }

        public IActionResult NotMaintainedItems()
        {
            return View();
        }

        public async Task<JsonResult> NotMaintainedItemsReport(QueryDto query)
        {
            var result = await _reportService.NotMaintainedItemsReport(query);
            return Json(result);
        }

        public IActionResult NotDeliveredItems()
        {
            return View();
        }

        public async Task<JsonResult> NotDeliveredItemsReport(QueryDto query)
        {
            var result = await _reportService.NotDeliveredItemsReport(query);
            return Json(result);
        }

        public IActionResult DeliveredItemsByTechnician()
        {
            return View();
        }

        public async Task<JsonResult> DeliveredItemsReportByTechnician(QueryDto query)
        {
            var result = await _reportService.DeliveredItemsReportByTechnician(query);
            return Json(result);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult CollectedAmounts()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<JsonResult> CollectedAmountsReport(QueryDto query)
        {
            var result = await _reportService.CollectedAmountsReport(query);
            return Json(result);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<double> CollectedAmountsReportTotal(QueryDto query)
        {
            var result = await _reportService.CollectedAmountsReportTotal(query);
            return result;
        }

        public IActionResult SuspendedItems()
        {
            return View();
        }

        public async Task<JsonResult> SuspendedItemsReport(QueryDto query)
        {
            var result = await _reportService.SuspendedItemsReport(query);
            return Json(result);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult TechnicianFees()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<JsonResult> TechnicianFeesReport(QueryDto query)
        {
            var result = await _reportService.TechnicianFeesReport(query);
            return Json(result);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<double> TechnicianFeesReportTotal(QueryDto query)
        {
            var result = await _reportService.TechnicianFeesReportTotal(query);
            return result;
        }

        public IActionResult RemovedFromMaintainedItems()
        {
            return View();
        }

        public async Task<JsonResult> RemovedFromMaintainedItemsReport(QueryDto query)
        {
            var result = await _reportService.RemovedFromMaintainedItemsReport(query);
            return Json(result);
        }

        public IActionResult MaintainedItems()
        {
            return View();
        }

        public async Task<JsonResult> MaintainedItemsReport(QueryDto query)
        {
            var result = await _reportService.MaintainedItemsReport(query);
            return Json(result);
        }
    }
}

