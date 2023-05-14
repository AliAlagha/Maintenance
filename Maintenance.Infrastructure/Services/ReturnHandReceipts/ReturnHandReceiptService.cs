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
using Maintenance.Core.Enums;
using Maintenance.Core.Helpers;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class ReturnHandReceiptService : IReturnHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ReturnHandReceiptService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ReturnHandReceiptViewModel>> GetAll(Pagination pagination
            , QueryDto query, string? barcode)
        {
            var dbQuery = _db.ReturnHandReceipts
                .Include(x => x.HandReceipt)
                .ThenInclude(x => x.Customer)
                .Include(x => x.ReceiptItems)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                dbQuery = dbQuery.Where(x => x.ReceiptItems.Any(x => x.ItemBarcode.Contains(barcode)));
            }

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.HandReceipt.Customer.Id == query.CustomerId);
            }

            return await dbQuery.ToPagedData<ReturnHandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateReturnHandReceiptDto input, string userId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == input.HandReceiptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var returnHandReceipt = _mapper.Map<ReturnHandReceipt>(input);
            await AddReturnHandReceiptItems(input, handReceipt, returnHandReceipt);

            returnHandReceipt.CustomerId = handReceipt.CustomerId;
            returnHandReceipt.CreatedBy = userId;
            await _db.ReturnHandReceipts.AddAsync(returnHandReceipt);
            await _db.SaveChangesAsync();
            return returnHandReceipt.Id;
        }

        private async Task AddReturnHandReceiptItems(CreateReturnHandReceiptDto input, HandReceipt? handReceipt, ReturnHandReceipt returnHandReceipt)
        {
            var selectedReturnHandReceiptItems = input.Items.Where(x => x.IsSelected).ToList();
            foreach (var returnHandReceiptItem in selectedReturnHandReceiptItems)
            {
                var handReceiptItem = handReceipt.ReceiptItems
                    .Single(x => x.Id == returnHandReceiptItem.HandReceiptItemId);

                var newReturnHandReceiptItem = new ReceiptItem
                {
                    CustomerId = handReceipt.CustomerId,
                    ReturnHandReceiptId = returnHandReceipt.Id,
                    Item = handReceiptItem.Item,
                    Color = handReceiptItem.Color,
                    Description = handReceiptItem.Description,
                    Company = handReceiptItem.Company,
                    ItemBarcode = await GenerateBarcode(),
                    WarrantyExpiryDate = handReceiptItem.WarrantyExpiryDate,
                    ReturnReason = returnHandReceiptItem.ReturnReason,
                    ReceiptItemType = ReceiptItemType.Returned
                };

                returnHandReceipt.ReceiptItems.Add(newReturnHandReceiptItem);
            }
        }

        public async Task Delete(int id, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.SingleOrDefaultAsync(x => x.Id == id);
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            returnHandReceipt.IsDelete = true;
            returnHandReceipt.UpdatedAt = DateTime.Now;
            returnHandReceipt.UpdatedBy = userId;
            _db.ReturnHandReceipts.Update(returnHandReceipt);
            await _db.SaveChangesAsync();
        }

        public async Task<List<HandReceiptItemForReturnViewModel>> GetHandReceiptItemsForReturn(int handReceiptId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            var itemVms = new List<HandReceiptItemForReturnViewModel>();

            for (int i = 0; i < handReceipt.ReceiptItems.Count; i++)
            {
                var handReceiptItem = handReceipt.ReceiptItems[i];
                itemVms.Add(new HandReceiptItemForReturnViewModel
                {
                    Index = i,
                    Id = handReceiptItem.Id,
                    Item = handReceiptItem.Item,
                    ItemBarcode = handReceiptItem.ItemBarcode,
                    Company = handReceiptItem.Company,
                    WarrantyExpiryDate = handReceiptItem.WarrantyExpiryDate != null
                        ? handReceiptItem.WarrantyExpiryDate.Value.ToString("yyyy-MM-dd") 
                        : null
                });
            }

            return itemVms;
        }

        // Heplers
        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExists = await _db.ReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
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

    }
}
