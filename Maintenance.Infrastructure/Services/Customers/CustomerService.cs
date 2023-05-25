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

namespace Maintenance.Infrastructure.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CustomerService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<CustomerViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Customers.OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.GeneralSearch)
                    || x.Email.Contains(query.GeneralSearch)
                    || x.PhoneNumber.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<CustomerViewModel>(pagination, _mapper);
        }

        public async Task<UpdateCustomerDto> Get(int id)
        {
            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateCustomerDto>(customer);
        }

        public async Task<AddRatingToCustomerDto> GetCustomerRating(int id)
        {
            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                throw new EntityNotFoundException();

            return _mapper.Map<AddRatingToCustomerDto>(customer);
        }

        public async Task<int> Create(CreateCustomerDto input, string userId)
        {
            var isCustomerExists = await _db.Customers.AnyAsync(x => x.PhoneNumber
                .Equals(input.PhoneNumber));
            if (isCustomerExists)
            {
                throw new AlreadyExistsException();
            }

            var customer = _mapper.Map<Customer>(input);
            customer.CreatedBy = userId;
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
            return customer.Id;
        }

        public async Task Update(UpdateCustomerDto input, string userId)
        {
            var isCustomerExists = await _db.Customers.AnyAsync(x => x.Id != input.Id
                && x.PhoneNumber.Equals(input.PhoneNumber));
            if (isCustomerExists)
            {
                throw new AlreadyExistsException();
            }

            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == input.Id);
            if (customer == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, customer);

            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userId;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == id);
            if (customer == null)
                throw new EntityNotFoundException();

            customer.IsDelete = true;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userId;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task AddRating(AddRatingToCustomerDto input, string userId)
        {
            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == input.Id);
            if (customer == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, customer);

            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = userId;
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BaseVm>> List()
        {
            var customers = _db.Customers.AsQueryable();

            return await customers.Select(x => new BaseVm
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
