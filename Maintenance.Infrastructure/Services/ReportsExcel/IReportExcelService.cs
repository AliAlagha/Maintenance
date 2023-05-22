using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ReportsExcel
{
    public interface IReportExcelService
    {
        Task<byte[]> ReceiptItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> ReturnedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> UrgentItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotDeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReportByTechnicianExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> CollectedAmountsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> SuspendedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> TechnicianFeesReportExcel(DateTime? dateFrom, DateTime? dateTo);
    }
}