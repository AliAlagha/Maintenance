using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ReportsPdf
{
    public interface IReportPdfService
    {
        Task<byte[]> ReceiptItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> DeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> ReturnedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> UrgentItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> NotMaintainedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> NotDeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> DeliveredItemsReportByTechnicianPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> CollectedAmountsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId);
        Task<byte[]> SuspendedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId);
        Task<byte[]> TechnicianFeesReportPdf(DateTime? dateFrom, DateTime? dateTo);
    }
}