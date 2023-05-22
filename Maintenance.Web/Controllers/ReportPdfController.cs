using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.ReportsPdf;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.Map.WebForms.BingMaps;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportPdfController : BaseController
    {
        private readonly IReportPdfService _reportPdfService;

        public ReportPdfController(IUserService userService
            , IReportPdfService reportPdfService) : base(userService)
        {
            _reportPdfService = reportPdfService;
        }

        public async Task<IActionResult> ReceiptItemsReportPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.ReceiptItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "ReceiptItemsReport");
        }

        public async Task<IActionResult> DeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.DeliveredItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "DeliveredItems");
        }

        public async Task<IActionResult> ReturnedItemsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportPdfService.ReturnedItemsReportPdf(dateFrom, dateTo, technicianId);
            return GetPdfFileResult(result, "ReturnedItems");
        }

        public async Task<IActionResult> UrgentItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.UrgentItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "UrgentItems");
        }

        public async Task<IActionResult> NotMaintainedItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.NotMaintainedItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "ReceiptItemsReport");
        }

        public async Task<IActionResult> NotDeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.NotDeliveredItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "NotDeliveredItems");
        }

        public async Task<IActionResult> DeliveredItemsReportByTechnician(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportPdfService.DeliveredItemsReportByTechnicianPdf(dateFrom, dateTo, technicianId);
            return GetPdfFileResult(result, "DeliveredItemsReportByTechnician");
        }

        public async Task<IActionResult> CollectedAmountsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportPdfService.CollectedAmountsReportPdf(dateFrom, dateTo, technicianId);
            return GetPdfFileResult(result, "CollectedAmountsReport");
        }

        public async Task<IActionResult> SuspendedItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.SuspendedItemsReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "SuspendedItemsReport");
        }

        public async Task<IActionResult> TechnicianFeesReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.TechnicianFeesReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "TechnicianFees");
        }
    }
}

