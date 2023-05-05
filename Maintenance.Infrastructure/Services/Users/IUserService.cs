using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Users
{
    public interface IUserService
    {
        Task<ResponseDto> GetAll(Pagination pagination, QueryDto query);
        Task<UpdateUserDto> Get(string id);
        User GetUserByUsername(string username);
        Task<string?> Create(CreateUserDto input, string userId);
        Task Update(UpdateUserDto input, string userId);
        Task Delete(string id, string userId);
        Task<string> ChangePassword(ChangePasswordDto dto);
        Task<bool> ChangePasswordForUser(ChangePasswordForUserDto dto, string userId);
        Task<bool> ActivationToggle(string id, string userId);
        Task<List<BaseUserVm>> List(UserType? userType);
    }
}