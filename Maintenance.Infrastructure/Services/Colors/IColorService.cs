using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Colors
{
    public interface IColorService
    {
        Task<PagingResultViewModel<ColorViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateColorDto> Get(int id);
        Task<int> Create(CreateColorDto input, string userId);
        Task Update(UpdateColorDto input, string userId);
        Task Delete(int id, string userId);
        Task<List<BaseVm>> List();
    }
}