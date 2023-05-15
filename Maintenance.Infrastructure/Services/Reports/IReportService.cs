using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.Reports
{
    public interface IReportService
    {
        Task<byte[]> ReceiptItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> DeliveredItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> ReturnedItemsReport(DateTime? fromDate, DateTime? toDate
            , string? technicianId);
        Task<byte[]> UrgentItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> NotMaintainedItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> NotDeliveredItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> DeliveredItemsReport(DateTime? fromDate, DateTime? toDate
            , string? technicianId);
        Task<byte[]> CollectedAmountsReport(DateTime? fromDate, DateTime? toDate
            , string? technicianId);
        Task<byte[]> SuspendedItemsReport(DateTime? fromDate, DateTime? toDate);
        Task<byte[]> TechnicianFeesReport(DateTime? fromDate, DateTime? toDate);
    }
}