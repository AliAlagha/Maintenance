using AutoMapper;
using Maintenance.Core.Constants;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Data.Extensions;
using Maintenance.Infrastructure.Extensions;
using Maintenance.Infrastructure.Services.Files;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Infrastructure.Services.BranchPhoneNumbers
{
    public class BranchPhoneNumberService : IBranchPhoneNumberService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public BranchPhoneNumberService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<BranchPhoneNumberViewModel>> GetAll(Pagination pagination
            , QueryDto query, int branchId)
        {
            var dbQuery = _db.BranchPhoneNumbers
                .Where(x => x.BranchId == branchId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.PhoneNumber.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<BranchPhoneNumberViewModel>(pagination, _mapper);
        }

        public async Task<UpdateBranchPhoneNumberDto> Get(int branchPhoneNumberId, int branchId)
        {
            var branchPhoneNumber = await _db.BranchPhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == branchPhoneNumberId 
                && x.BranchId == branchId);
            if (branchPhoneNumber == null)
                throw new EntityNotFoundException();

            var dto = _mapper.Map<UpdateBranchPhoneNumberDto>(branchPhoneNumber);
            return dto;
        }

        public async Task<int> Create(CreateBranchPhoneNumberDto input, string userId)
        {
            var branchPhoneNumber = _mapper.Map<BranchPhoneNumber>(input);
            branchPhoneNumber.CreatedBy = userId;
            await _db.BranchPhoneNumbers.AddAsync(branchPhoneNumber);
            await _db.SaveChangesAsync();
            return branchPhoneNumber.Id;
        }

        public async Task Update(UpdateBranchPhoneNumberDto input, string userId)
        {
            var branchPhoneNumber = await _db.BranchPhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == input.BranchPhoneNumberId 
                && x.BranchId == input.BranchId);
            if (branchPhoneNumber == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, branchPhoneNumber);
            branchPhoneNumber.UpdatedAt = DateTime.Now;
            branchPhoneNumber.UpdatedBy = userId;
            _db.BranchPhoneNumbers.Update(branchPhoneNumber);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int branchPhoneNumberId, int branchId, string userId)
        {
            var branchPhoneNumber = await _db.BranchPhoneNumbers
                .SingleOrDefaultAsync(x => x.Id == branchPhoneNumberId
                && x.BranchId == branchId);
            if (branchPhoneNumber == null)
                throw new EntityNotFoundException();

            branchPhoneNumber.IsDelete = true;
            branchPhoneNumber.UpdatedAt = DateTime.Now;
            branchPhoneNumber.UpdatedBy = userId;
            _db.BranchPhoneNumbers.Update(branchPhoneNumber);
            await _db.SaveChangesAsync();
        }

    }
}
