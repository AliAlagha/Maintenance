using AutoMapper;
using Maintenance.Core.Constants;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Data.Extensions;
using Maintenance.Infrastructure.Services.Files;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UserService(ApplicationDbContext db, IMapper mapper, UserManager<User> userManager
            , IFileService fileService)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<ResponseDto> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Users.Include(x => x.Branch)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.FullName.Contains(query.GeneralSearch)
                    || x.Email.Contains(query.GeneralSearch)
                    || x.PhoneNumber.Contains(query.GeneralSearch));
            }

            if (query.BranchId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var dataCount = dbQuery.Count();
            var skipValue = (pagination.Page - 1) * pagination.PerPage;
            var result = await dbQuery.Skip(skipValue).Take(pagination.PerPage).ToListAsync();
            var dataList = _mapper.Map<List<UserViewModel>>(result);
            var pages = Convert.ToInt32(Math.Ceiling(dataCount / (float)pagination.PerPage));

            return new ResponseDto
            {
                data = dataList,
                meta = new Meta
                {
                    page = pagination.Page,
                    perpage = pagination.PerPage,
                    pages = pages,
                    total = dataCount,
                }
            };
        }

        public async Task<UpdateUserDto> Get(string id)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateUserDto>(user);
        }

        public User GetUserByUsername(string username)
        {
            var user = _db.Users.SingleOrDefault(x => x.UserName == username);
            if (user == null)
            {
                throw new EntityNotFoundException();
            }

            return user;
        }

        public async Task<string?> Create(CreateUserDto input, string userId)
        {
            string? createdUserId = null;
            await TransactionExtension.UseTransaction(_db, async () =>
            {
                var isEmailExists = await _db.Users.AnyAsync(x => x.Email.ToLower()
                    .Equals(input.Email.ToLower()));
                if (isEmailExists)
                    throw new EmailAlreadyExistsException();

                var user = _mapper.Map<User>(input);

                if (input.ImageFile != null)
                    user.ImageFilePath = await _fileService.SaveFile(input.ImageFile, FolderNames.Images);

                user.CreatedBy = userId;
                user.UserName = input.Email;
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                var createUserResult = await _userManager.CreateAsync(user, input.Password);
                if (!createUserResult.Succeeded)
                {
                    throw new OperationFailedException();
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user, input.UserType.ToString());
                if (!addUserToRoleResult.Succeeded)
                {
                    throw new OperationFailedException();
                }

                createdUserId = user.Id;
            });

            return createdUserId;
        }

        public async Task Update(UpdateUserDto input, string userId)
        {
            await TransactionExtension.UseTransaction(_db, async () =>
            {
                var isEmailExists = await _db.Users.AnyAsync(x => x.Id != input.Id
                && x.Email.ToLower().Equals(input.Email.ToLower()));
                if (isEmailExists)
                    throw new EmailAlreadyExistsException();

                var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == input.Id);
                if (user == null)
                    throw new EntityNotFoundException();

                var currentUserRole = user.UserType.ToString();

                _mapper.Map(input, user);
                if (input.ImageFile != null)
                    user.ImageFilePath = await _fileService.SaveFile(input.ImageFile, FolderNames.Images);

                user.UserName = user.Email;
                var updateUserResult = await _userManager.UpdateAsync(user);
                if (!updateUserResult.Succeeded)
                {
                    throw new OperationFailedException();
                }

                await UpdateUserRole(input, user, currentUserRole);
                await ChangeUserPassword(input, user);
            });
        }

        private async Task UpdateUserRole(UpdateUserDto input, User? user, string currentUserRole)
        {
            if (!currentUserRole.Equals(input.UserType.ToString()))
            {
                var removeUserFromRoleResult = await _userManager.RemoveFromRoleAsync(user, currentUserRole);
                if (!removeUserFromRoleResult.Succeeded)
                {
                    throw new OperationFailedException();
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user, user.UserType.ToString());
                if (!addUserToRoleResult.Succeeded)
                {
                    throw new OperationFailedException();
                }
            }
        }

        private async Task ChangeUserPassword(UpdateUserDto input, User? user)
        {
            if (!string.IsNullOrWhiteSpace(input.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, input.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    throw new OperationFailedException();
                }
            }
        }

        public async Task Delete(string id, string userId)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new EntityNotFoundException();

            user.IsDelete = true;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = userId;
            await _userManager.UpdateAsync(user);
        }

        public async Task<string> ChangePassword(ChangePasswordDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == dto.Id);
            if (user == null)
            {
                throw new EntityNotFoundException();
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, dto.Password);

            return user.Id;
        }

        public async Task<bool> ChangePasswordForUser(ChangePasswordForUserDto dto, string userId)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id.Equals(userId));
            if (user == null)
            {
                throw new EntityNotFoundException();
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task<bool> ActivationToggle(string id, string userId)
        {
            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new EntityNotFoundException();

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = userId;
            await _userManager.UpdateAsync(user);
            return user.IsActive;
        }

        public async Task<List<BaseUserVm>> List(UserType? userType)
        {
            var users = _db.Users.AsQueryable();

            if (userType.HasValue)
            {
                users = users.Where(x => x.UserType == userType);
            }

            return await users.Select(x => new BaseUserVm
            {
                Id = x.Id,
                Name = x.FullName
            }).ToListAsync();
        }
    }
}
