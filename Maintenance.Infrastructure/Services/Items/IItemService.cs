using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Items
{
    public interface IItemService
    {
        Task<PagingResultViewModel<ItemViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateItemDto> Get(int id);
        Task<int> Create(CreateItemDto input, string userId);
        Task Update(UpdateItemDto input, string userId);
        Task Delete(int id, string userId);
        Task<List<BaseVm>> List();
    }
}