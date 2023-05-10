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

namespace Maintenance.Infrastructure.Services.HandReceiptItems
{
    public class HandReceiptItemService : IHandReceiptItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public HandReceiptItemService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<HandReceiptItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int handReceiptId)
        {
            var dbQuery = _db.HandReceiptItems
                .Where(x => x.HandReceiptId == handReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<HandReceiptItemViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateHandReceiptItemDto input, string userId)
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
            var isBarcodeExistsInHandReceipt = await _db.HandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExistsInHandReceipt)
            {
                await GenerateBarcode();
            }

            var isBarcodeExistsInReturnHandReceipt = await _db.ReturnHandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExistsInReturnHandReceipt)
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

        public async Task Delete(int handReceiptItemId, int handReceiptId, string userId)
        {
            var handReceiptItem = await _db.HandReceiptItems.SingleOrDefaultAsync(x => x.Id == handReceiptItemId
                && x.HandReceiptId == handReceiptId);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.IsDelete = true;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task CollectMoney(CollectMoneyForHandReceiptItemDto dto, string userId)
        {
            var handReceiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.HandReceiptItemId
                && x.HandReceiptId == dto.HandReceiptId && x.CollectedAmount == null);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.CollectedAmount = dto.CollectedAmount;
            handReceiptItem.CollectionDate = DateTime.Now;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int handReceiptItemId, int handReceiptId, string userId)
        {
            var handReceiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == handReceiptItemId && x.HandReceiptId == handReceiptId
                && x.Delivered == false);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.Delivered = true;
            handReceiptItem.DeliveryDate = DateTime.Now;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int handReceiptId, string userId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.HandReceiptItems.Where(x => !x.Delivered))
                .SingleOrDefaultAsync(x => x.Id == handReceiptId
                && !x.HandReceiptItems.All(x => x.Delivered));
            if (handReceipt == null)
                throw new EntityNotFoundException();

            foreach (var handReceiptItem in handReceipt.HandReceiptItems)
            {
                handReceiptItem.Delivered = true;
                handReceiptItem.DeliveryDate = DateTime.Now;
                handReceiptItem.UpdatedAt = DateTime.Now;
                handReceiptItem.UpdatedBy = userId;
                _db.HandReceiptItems.Update(handReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAllItemsDelivered(int handReceiptId)
        {
            var returnHandReceipt = await _db.HandReceipts.Include(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsDelivered = returnHandReceipt.HandReceiptItems.All(x => x.Delivered);
            return isAllItemsDelivered;
        }

    }
}
