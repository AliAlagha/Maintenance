using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.Maintenance
{
    public interface IMaintenanceService
    {
        Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, string userId);
        Task UpdateStatus(int receiptItemId, string userId);
        Task CustomerRefuseMaintenance(CustomerRefuseMaintenanceDto dto, string userId);
        Task SuspenseMaintenance(SuspenseReceiptItemDto dto, string userId);
        Task EnterMaintenanceCost(EnterMaintenanceCostDto dto, string userId);
    }
}