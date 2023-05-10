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

namespace Maintenance.Infrastructure.Services.ReturnHandReceiptItems
{
    public class ReturnHandReceiptItemService : IReturnHandReceiptItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ReturnHandReceiptItemService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int returnHandReceiptId)
        {
            var dbQuery = _db.ReturnHandReceiptItems
                .Where(x => x.ReturnHandReceiptId == returnHandReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ReturnHandReceiptItemViewModel>(pagination, _mapper);
        }

        public async Task Delete(int returnHandReceiptItemId, int returnHandReceiptId, string userId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptItemId
                && x.ReturnHandReceiptId == returnHandReceiptId);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.IsDelete = true;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update((ReturnHandReceiptItem)returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int returnHandReceiptItemId, int returnHandReceiptId, string userId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptItemId
                && x.ReturnHandReceiptId == returnHandReceiptId && x.Delivered == false);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.Delivered = true;
            returnHandReceiptItem.DeliveryDate = DateTime.Now;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int returnHandReceiptId, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.ReturnHandReceiptItems.Where(x => !x.Delivered))
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId
                && !x.ReturnHandReceiptItems.All(x => x.Delivered));
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            foreach (var returnHandReceiptItem in returnHandReceipt.ReturnHandReceiptItems)
            {
                returnHandReceiptItem.Delivered = true;
                returnHandReceiptItem.DeliveryDate = DateTime.Now;
                returnHandReceiptItem.UpdatedAt = DateTime.Now;
                returnHandReceiptItem.UpdatedBy = userId;
                _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAllItemsDelivered(int returnHandReceiptId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.Include(x => x.ReturnHandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsDelivered = returnHandReceipt.ReturnHandReceiptItems.All(x => x.Delivered);
            return isAllItemsDelivered;
        }

    }
}
