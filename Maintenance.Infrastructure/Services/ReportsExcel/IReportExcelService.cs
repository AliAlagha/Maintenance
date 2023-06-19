using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ReportsExcel
{
    public interface IReportExcelService
    {
        Task<byte[]> ReceiptItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> DeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> ReturnedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> UrgentItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> NotMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> NotDeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> DeliveredItemsReportByTechnicianExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> CollectedAmountsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> SuspendedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> TechnicianFeesReportExcel(DateTime? dateFrom, DateTime? dateTo, string? technicianId, int? branchId);
        Task<byte[]> RemovedFromMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
    }
}