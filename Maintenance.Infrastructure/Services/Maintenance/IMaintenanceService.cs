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
        Task UpdateStatusForHandReceiptItem(int receiptItemId, string userId);
        Task CustomerRefuseMaintenanceForHandReceiptItem(CustomerRefuseMaintenanceDto dto, string userId);
        Task SuspenseMaintenanceForHandReceiptItem(SuspenseReceiptItemDto dto, string userId);
        Task EnterMaintenanceCostForHandReceiptItem(EnterMaintenanceCostDto dto, string userId);

        // Return hand receipt items
        Task UpdateStatusForReturnHandReceiptItem(int receiptItemId, string userId);
        Task SuspenseMaintenanceForReturnHandReceiptItem(SuspenseReceiptItemDto dto, string userId);
    }
}