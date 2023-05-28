using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.RecipientMaintenances
{
    public interface IRecipientMaintenanceService
    {
        Task<PagingResultViewModel<RecipientMaintenanceViewModel>> GetAll
            (Pagination pagination, QueryDto query);
        Task<int> Create(CreateRecipientMaintenanceDto input, string userId);
        Task Delete(int id, string userId);
    }
}