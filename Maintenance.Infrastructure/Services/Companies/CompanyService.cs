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

namespace Maintenance.Infrastructure.Services.Companies
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CompanyService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<CompanyViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Companies.OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.GeneralSearch)
                    || x.Email.Contains(query.GeneralSearch)
                    || x.PhoneNumber.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<CompanyViewModel>(pagination, _mapper);
        }

        public async Task<UpdateCompanyDto> Get(int id)
        {
            var company = await _db.Companies.SingleOrDefaultAsync(x => x.Id == id);
            if (company == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateCompanyDto>(company);
        }

        public async Task<int> Create(CreateCompanyDto input, string userId)
        {
            var isExists = _db.Companies.Any(x => x.Name.Equals(input.Name));
            if (isExists)
            {
                throw new EntityAlreadyExistsException();
            }

            var company = _mapper.Map<Company>(input);
            company.CreatedBy = userId;
            await _db.Companies.AddAsync(company);
            await _db.SaveChangesAsync();
            return company.Id;
        }

        public async Task Update(UpdateCompanyDto input, string userId)
        {
            var company = await _db.Companies.SingleOrDefaultAsync(x => x.Id == input.Id);
            if (company == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, company);

            company.UpdatedAt = DateTime.Now;
            company.UpdatedBy = userId;
            _db.Companies.Update(company);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var company = await _db.Companies.SingleOrDefaultAsync(x => x.Id == id);
            if (company == null)
                throw new EntityNotFoundException();

            company.IsDelete = true;
            company.UpdatedAt = DateTime.Now;
            company.UpdatedBy = userId;
            _db.Companies.Update(company);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BaseVm>> List()
        {
            var companies = _db.Companies.AsQueryable();

            return await companies.Select(x => new BaseVm
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
