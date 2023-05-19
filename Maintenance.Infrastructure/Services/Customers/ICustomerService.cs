using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Customers
{
    public interface ICustomerService
    {
        Task<PagingResultViewModel<CustomerViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateCustomerDto> Get(int id);
        Task<AddRatingToCustomerDto> GetCustomerRating(int id);
        Task<int> Create(CreateCustomerDto input, string userId);
        Task Update(UpdateCustomerDto input, string userId);
        Task Delete(int id, string userId);
        Task AddRating(AddRatingToCustomerDto input, string userId);
        Task<List<BaseVm>> List();
    }
}