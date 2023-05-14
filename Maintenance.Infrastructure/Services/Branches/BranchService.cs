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

namespace Maintenance.Infrastructure.Services.Branches
{
    public class BranchService : IBranchService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public BranchService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<BranchViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Branches
                .Include(x => x.BranchPhoneNumbers)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<BranchViewModel>(pagination, _mapper);
        }

        public async Task<UpdateBranchDto> Get(int id)
        {
            var branch = await _db.Branches
                .SingleOrDefaultAsync(x => x.Id == id);
            if (branch == null)
                throw new EntityNotFoundException();

            var dto = _mapper.Map<UpdateBranchDto>(branch);
            return dto;
        }

        public async Task<int> Create(CreateBranchDto input, string userId)
        {
            var branch = _mapper.Map<Branch>(input);
            branch.CreatedBy = userId;
            await _db.Branches.AddAsync(branch);
            await _db.SaveChangesAsync();
            return branch.Id;
        }

        public async Task Update(UpdateBranchDto input, string userId)
        {
            var branch = await _db.Branches
                .SingleOrDefaultAsync(x => x.Id == input.Id);
            if (branch == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, branch);
            branch.UpdatedAt = DateTime.Now;
            branch.UpdatedBy = userId;
            _db.Branches.Update(branch);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == id);
            if (branch == null)
                throw new EntityNotFoundException();

            branch.IsDelete = true;
            branch.UpdatedAt = DateTime.Now;
            branch.UpdatedBy = userId;
            _db.Branches.Update(branch);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BaseVm>> List()
        {
            var branchs = _db.Branches.AsQueryable();

            return await branchs.Select(x => new BaseVm
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
