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
            var dbQuery = _db.HandReceipts.Include(x => x.Customer)
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

        // Items
        public async Task<PagingResultViewModel<HandReceiptItemViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, int handReceiptId)
        {
            var dbQuery = _db.HandReceiptItems
                .Where(x => x.HandReceiptId == handReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            return await dbQuery.ToPagedData<HandReceiptItemViewModel>(pagination, _mapper);
        }

        public async Task<int> CreateHandReceiptItem(CreateHandReceiptItemDto input, string userId)
        {
            var handReceiptItem = _mapper.Map<HandReceiptItem>(input);

            var handReceipt = await _db.HandReceipts.SingleOrDefaultAsync(x => x.Id == input.HandReceiptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var item = await _db.Items.SingleOrDefaultAsync(x => x.Id == input.ItemId);
            if (item == null)
            {
                throw new EntityNotFoundException();
            }

            var company = await _db.Companies.SingleOrDefaultAsync(x => x.Id == input.CompanyId);
            if (company == null)
            {
                throw new EntityNotFoundException();
            }

            if (input.ColorId != null)
            {
                var color = await _db.Colors.SingleOrDefaultAsync(x => x.Id == input.ColorId);
                if (color == null)
                {
                    throw new EntityNotFoundException();
                }

                handReceiptItem.Color = color.Name;
            }

            if (input.SpecifiedCost == null)
            {
                handReceiptItem.NotifyCustomerOfTheCost = true;
            }

            handReceiptItem.Item = item.Name;
            handReceiptItem.Company = company.Name;
            handReceiptItem.ItemBarcode = await GenerateBarcode();
            handReceiptItem.CreatedBy = userId;
            await _db.HandReceiptItems.AddAsync(handReceiptItem);
            await _db.SaveChangesAsync();
            return handReceiptItem.Id;
        }

        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExists = await _db.HandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExists)
            {
                await GenerateBarcode();
            }

            return barcode;
        }

        private string RandomDigits(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = string.Concat(s, random.Next(10).ToString());
            return s;
        }

        public async Task DeleteHandReceiptItem(int id, string userId)
        {
            var handReceiptItem = await _db.HandReceiptItems.SingleOrDefaultAsync(x => x.Id == id);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.IsDelete = true;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

    }
}
