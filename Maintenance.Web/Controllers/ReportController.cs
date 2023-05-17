using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
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

        public async Task<IActionResult> ExportReceiptItemsToPdf(DateTime? dateFrom
            , DateTime? dateTo)
        {
            var result = await _reportService.ReceiptItemsReport(dateFrom, dateTo);
            return GetPdfFileResult(result, $"{DateTime.Now:yyyy-MM-dd} - ReceiptItems");
        }

        public IActionResult DeliveredItems()
        {
            return View();
        }

        public async Task<IActionResult> DeliveredItemsReport(DateTime? dateFrom
            , DateTime? dateTo)
        {
            var result = await _reportService.DeliveredItemsReport(dateFrom, dateTo);
            return GetPdfFileResult(result, $"{DateTime.Now:yyyy-MM-dd} - ReceiptItems");
        }

        public IActionResult ReturnedItems()
        {
            return View();
        }

        public async Task<IActionResult> ReturnedItemsReport(DateTime? dateFrom
            , DateTime? dateTo, string? technicianId)
        {
            var result = await _reportService.ReturnedItemsReport(dateFrom, dateTo, technicianId);
            return GetPdfFileResult(result, $"{DateTime.Now:yyyy-MM-dd} - ReceiptItems");
        }

        public IActionResult UrgentItems()
        {
            return View();
        }

        public async Task<IActionResult> UrgentItemsReport(DateTime? dateFrom
            , DateTime? dateTo)
        {
            var result = await _reportService.UrgentItemsReport(dateFrom, dateTo);
            return GetPdfFileResult(result, $"{DateTime.Now:yyyy-MM-dd} - UrgentItems");
        }
    }
}

