using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.BranchPhoneNumbers
{
    public interface IBranchPhoneNumberService
    {
        Task<PagingResultViewModel<BranchPhoneNumberViewModel>> GetAll(Pagination pagination
            , QueryDto query, int branchId);
        Task<UpdateBranchPhoneNumberDto> Get(int branchPhoneNumberId, int branchId);
        Task<int> Create(CreateBranchPhoneNumberDto input, string userId);
        Task Update(UpdateBranchPhoneNumberDto input, string userId);
        Task Delete(int branchPhoneNumberId, int branchId, string userId);
    }
}