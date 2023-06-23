using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.Reports
{
    public interface IReportService
    {
        Task<List<ReceiptItemReportDataSet>> ReceiptItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> DeliveredItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> ReturnedItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> UrgentItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> NotMaintainedItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> NotDeliveredItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> DeliveredItemsReportByTechnician(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> CollectedAmountsReport(QueryDto query);
        Task<double> CollectedAmountsReportTotal(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> SuspendedItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> TechnicianFeesReport(QueryDto query);
        Task<double> TechnicianFeesReportTotal(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> RemovedFromMaintainedItemsReport(QueryDto query);
        Task<List<ReceiptItemReportDataSet>> MaintainedItemsReport(QueryDto query);
    }
}