using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Data.Extensions;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class HandReceiptService : IHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;

        public HandReceiptService(ApplicationDbContext db, IMapper mapper
            , ICustomerService customerService)
        {
            _db = db;
            _mapper = mapper;
            _customerService = customerService;
        }

        public async Task<PagingResultViewModel<HandReceiptViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.HandReceipts
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItems)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.Email.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<HandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<int?> Create(CreateHandReceiptDto input, string userId)
        {
            int? id = null;
            await _db.UseTransaction(async () =>
            {
                var handReceipt = _mapper.Map<HandReceipt>(input);

                if (input.CustomerId == null && input.CustomerInfo != null)
                {
                    var createCustomerDto = _mapper.Map<CreateCustomerForHandReceiptDto
                        , CreateCustomerDto>(input.CustomerInfo);
                    handReceipt.CustomerId = await _customerService.Create(createCustomerDto, userId);
                }

                handReceipt.Date = DateTime.Now;
                handReceipt.CreatedBy = userId;
                await _db.HandReceipts.AddAsync(handReceipt);
                await _db.SaveChangesAsync();
                id = handReceipt.Id;
            });

            return id;
        }

        public async Task Delete(int id, string userId)
        {
            var handReceipt = await _db.HandReceipts.SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            handReceipt.IsDelete = true;
            handReceipt.UpdatedAt = DateTime.Now;
            handReceipt.UpdatedBy = userId;
            _db.HandReceipts.Update(handReceipt);
            await _db.SaveChangesAsync();
        }

    }
}
