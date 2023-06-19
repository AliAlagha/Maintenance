using Maintenance.Infrastructure.Services.ReportsExcel;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ReportExcelController : BaseController
    {
        private readonly IReportExcelService _reportExcelService;

        public ReportExcelController(IUserService userService
            , IReportExcelService reportExcelService) : base(userService)
        {
            _reportExcelService = reportExcelService;
        }

        public async Task<IActionResult> ReceiptItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.ReceiptItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "ReceiptItems");
        }

        public async Task<IActionResult> DeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.DeliveredItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "DeliveredItems");
        }

        public async Task<IActionResult> ReturnedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportExcelService.ReturnedItemsReportExcel(dateFrom, dateTo, technicianId, branchId);
            return GetExcelFileResult(result, "ReturnedItems");
        }

        public async Task<IActionResult> UrgentItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.UrgentItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "UrgentItems");
        }
        public async Task<IActionResult> NotMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.NotMaintainedItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "NotMaintainedItems");
        }

        public async Task<IActionResult> NotDeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.NotDeliveredItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "NotDeliveredItems");
        }

        public async Task<IActionResult> DeliveredItemsReportByTechnicianExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportExcelService.DeliveredItemsReportByTechnicianExcel(dateFrom, dateTo, technicianId, branchId);
            return GetExcelFileResult(result, "DeliveredItemsReportByTechnician");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CollectedAmountsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportExcelService.CollectedAmountsReportExcel(dateFrom, dateTo, technicianId, branchId);
            return GetExcelFileResult(result, "CollectedAmounts");
        }

        public async Task<IActionResult> SuspendedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var result = await _reportExcelService.SuspendedItemsReportExcel(dateFrom, dateTo, branchId);
            return GetExcelFileResult(result, "SuspendedItems");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> TechnicianFeesReportExcel(DateTime? dateFrom, DateTime? dateTo, string? technicianId, int? branchId)
        {
            var result = await _reportExcelService.TechnicianFeesReportExcel(dateFrom, dateTo, technicianId, branchId);
            return GetExcelFileResult(result, "TechnicianFees");
        }

        public async Task<IActionResult> RemovedFromMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var result = await _reportExcelService.RemovedFromMaintainedItemsReportExcel(dateFrom, dateTo, technicianId, branchId);
            return GetExcelFileResult(result, "RemovedFromMaintainedItems");
        }
    }
}

