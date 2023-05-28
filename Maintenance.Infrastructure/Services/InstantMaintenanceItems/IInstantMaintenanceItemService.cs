using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceiptItems
{
    public interface IInstantMaintenanceItemService
    {
        Task<PagingResultViewModel<InstantMaintenanceItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int instantMaintenanceId);
        Task<int> Create(CreateItemForExistsInstantMaintenanceDto input, string userId);
        Task Delete(int instantMaintenanceItemId, int instantMaintenanceId, string userId);
    }
}