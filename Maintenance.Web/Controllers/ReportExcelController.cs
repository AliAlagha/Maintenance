using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.ReportsExcel;
using Maintenance.Infrastructure.Services.ReportsExcel;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.Map.WebForms.BingMaps;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportExcelController : BaseController
    {
        private readonly IReportExcelService _reportExcelService;

        public ReportExcelController(IUserService userService
            , IReportExcelService reportExcelService) : base(userService)
        {
            _reportExcelService = reportExcelService;
        }

        public async Task<IActionResult> ReceiptItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.ReceiptItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "ReceiptItems");
        }

        public async Task<IActionResult> DeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.DeliveredItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "DeliveredItems");
        }

        public async Task<IActionResult> ReturnedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportExcelService.ReturnedItemsReportExcel(dateFrom, dateTo, technicianId);
            return GetExcelFileResult(result, "ReturnedItems");
        }

        public async Task<IActionResult> UrgentItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.UrgentItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "UrgentItems");
        }
        public async Task<IActionResult> NotMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.NotMaintainedItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "NotMaintainedItems");
        }

        public async Task<IActionResult> NotDeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.NotDeliveredItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "NotDeliveredItems");
        }

        public async Task<IActionResult> DeliveredItemsReportByTechnicianExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportExcelService.DeliveredItemsReportByTechnicianExcel(dateFrom, dateTo, technicianId);
            return GetExcelFileResult(result, "DeliveredItemsReportByTechnician");
        }

        public async Task<IActionResult> CollectedAmountsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var result = await _reportExcelService.CollectedAmountsReportExcel(dateFrom, dateTo, technicianId);
            return GetExcelFileResult(result, "CollectedAmounts");
        }

        public async Task<IActionResult> SuspendedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.SuspendedItemsReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "SuspendedItems");
        }

        public async Task<IActionResult> TechnicianFeesReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var result = await _reportExcelService.TechnicianFeesReportExcel(dateFrom, dateTo);
            return GetExcelFileResult(result, "TechnicianFees");
        }
    }
}

