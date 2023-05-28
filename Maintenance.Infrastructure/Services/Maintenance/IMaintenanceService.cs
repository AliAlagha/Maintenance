using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.Maintenance
{
    public interface IMaintenanceService
    {
        Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllHandReceiptItems(Pagination pagination
            , QueryDto query, string userId);
        Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllReturnHandReceiptItems(Pagination pagination
            , QueryDto query, string userId);

        // Hand receipt items
        Task UpdateStatusForHandReceipt(int receiptItemId, string userId);
        Task CustomerRefuseMaintenanceForHandReceipt(CustomerRefuseMaintenanceDto dto, string userId);
        Task SuspenseMaintenanceForHandReceipt(SuspenseReceiptItemDto dto, string userId);
        Task EnterMaintenanceCostForHandReceipt(EnterMaintenanceCostDto dto, string userId);

        // Return hand receipt items
        Task UpdateStatusForReturnHandReceipt(int receiptItemId, string userId);
        Task SuspenseMaintenanceForReturnHandReceipt(SuspenseReceiptItemDto dto, string userId);
    }
}