using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.Reports
{
    public interface IReportService
    {
        Task<byte[]> ReceiptItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> ReturnedItemsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> UrgentItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotMaintainedItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotDeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> CollectedAmountsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> SuspendedItemsReport(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> TechnicianFeesReport(DateTime? dateFrom, DateTime? dateTo);
    }
}