using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Companies
{
    public interface ICompanyService
    {
        Task<PagingResultViewModel<CompanyViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateCompanyDto> Get(int id);
        Task<int> Create(CreateCompanyDto input, string userId);
        Task Update(UpdateCompanyDto input, string userId);
        Task Delete(int id, string userId);
        Task<List<BaseVm>> List();
    }
}