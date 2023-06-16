using Maintenance.Infrastructure.Services.ReportsPdf;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ReportPdfController : BaseController
    {
        private readonly IReportPdfService _reportPdfService;

        public ReportPdfController(IUserService userService
            , IReportPdfService reportPdfService) : base(userService)
        {
            _reportPdfService = reportPdfService;
        }

        public async Task<IActionResult> ReceiptItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.ReceiptItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "ReceiptItemsReport");
        }

        public async Task<IActionResult> DeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.DeliveredItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "DeliveredItems");
        }

        public async Task<IActionResult> ReturnedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportPdfService.ReturnedItemsReportPdf(dateFrom, dateTo, technicianId, branchId);
            return GetPdfFileResult(result, "ReturnedItems");
        }

        public async Task<IActionResult> UrgentItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.UrgentItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "UrgentItems");
        }

        public async Task<IActionResult> NotMaintainedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.NotMaintainedItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "ReceiptItemsReport");
        }

        public async Task<IActionResult> NotDeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.NotDeliveredItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "NotDeliveredItems");
        }

        public async Task<IActionResult> DeliveredItemsReportByTechnicianPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportPdfService.DeliveredItemsReportByTechnicianPdf(dateFrom, dateTo, technicianId, branchId);
            return GetPdfFileResult(result, "DeliveredItemsReportByTechnician");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CollectedAmountsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportPdfService.CollectedAmountsReportPdf(dateFrom, dateTo, technicianId, branchId);
            return GetPdfFileResult(result, "CollectedAmountsReport");
        }

        public async Task<IActionResult> SuspendedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportPdfService.SuspendedItemsReportPdf(dateFrom, dateTo, branchId);
            return GetPdfFileResult(result, "SuspendedItemsReport");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> TechnicianFeesReportPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportPdfService.TechnicianFeesReportPdf(dateFrom, dateTo);
            return GetPdfFileResult(result, "TechnicianFees");
        }

        public async Task<IActionResult> RemovedFromMaintainedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportPdfService.RemovedFromMaintainedItemsReportPdf(UserId, dateFrom, dateTo, technicianId, branchId);
            return GetPdfFileResult(result, "RemovedFromMaintainedItems");
        }
    }
}

