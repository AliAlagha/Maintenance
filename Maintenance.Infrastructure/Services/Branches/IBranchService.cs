using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Branches
{
    public interface IBranchService
    {
        Task<PagingResultViewModel<BranchViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateBranchDto> Get(int id);
        Task<int> Create(CreateBranchDto input, string userId);
        Task Update(UpdateBranchDto input, string userId);
        Task Delete(int id, string userId);
        Task<List<BaseVm>> List();
    }
}