using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ReportsPdf
{
    public interface IReportPdfService
    {
        Task<byte[]> ReceiptItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> ReturnedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> UrgentItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotMaintainedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> NotDeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> DeliveredItemsReportByTechnicianPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> CollectedAmountsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId);
        Task<byte[]> SuspendedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo);
        Task<byte[]> TechnicianFeesReportPdf(DateTime? dateFrom, DateTime? dateTo);
    }
}