using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public interface IInstantMaintenanceService
    {
        Task<PagingResultViewModel<InstantMaintenanceViewModel>> GetAll
            (Pagination pagination, QueryDto query);
        Task<int?> Create(CreateInstantMaintenanceDto input, string userId);
        Task Delete(int id, string userId);
    }
}