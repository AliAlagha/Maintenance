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
using Maintenance.Core.Resources;

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
            var dbQuery = _db.ReceiptItems
                .Where(x => x.HandReceiptId == handReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<HandReceiptItemViewModel>> ItemsPagedData(IQueryable<ReceiptItem> query
            , Pagination dto)
        {
            var pageSize = dto.PerPage;
            var skip = (int)Math.Ceiling(pageSize * (decimal)(dto.Page - 1));

            var totalCount = await query.CountAsync();
            query = query.Skip(skip).Take(pageSize);

            var items = await query.ToListAsync();

            var itemVms = new List<HandReceiptItemViewModel>();
            foreach (var item in items)
            {
                var itemVm = _mapper.Map<HandReceiptItemViewModel>(item);

                switch (item.MaintenanceRequestStatus)
                {
                    case MaintenanceRequestStatus.New:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                        break;
                    case MaintenanceRequestStatus.Suspended:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                        break;
                    case MaintenanceRequestStatus.CustomerRefused:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerRefused} - {item.ReasonForRefusingMaintenance}";
                        break;
                    case MaintenanceRequestStatus.Completed:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Completed}";
                        break;
                    case MaintenanceRequestStatus.Delivered:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Delivered}";
                        break;
                };

                itemVms.Add(itemVm);
            }

            return new PagingResultViewModel<HandReceiptItemViewModel>
            {
                Meta = new MetaViewModel
                {
                    Page = dto.Page,
                    Perpage = dto.PerPage,
                    Total = totalCount
                },
                Data = itemVms
            };
        }

        public async Task<int> Create(CreateHandReceiptItemDto input, string userId)
        {
            var handReceiptItem = _mapper.Map<ReceiptItem>(input);

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

            handReceiptItem.CustomerId = handReceipt.CustomerId;
            handReceiptItem.Item = item.Name;
            handReceiptItem.Company = company.Name;
            handReceiptItem.ItemBarcode = await GenerateBarcode();
            handReceiptItem.ReceiptItemType = ReceiptItemType.New;
            handReceiptItem.CreatedBy = userId;
            await _db.ReceiptItems.AddAsync(handReceiptItem);
            await _db.SaveChangesAsync();
            return handReceiptItem.Id;
        }

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

        public async Task Delete(int handReceiptItemId, int handReceiptId, string userId)
        {
            var handReceiptItem = await _db.ReceiptItems.SingleOrDefaultAsync(x => x.Id == handReceiptItemId
                && x.HandReceiptId == handReceiptId);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.IsDelete = true;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task CollectMoney(CollectMoneyForHandReceiptItemDto dto, string userId)
        {
            var handReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.HandReceiptItemId
                && x.HandReceiptId == dto.HandReceiptId && x.CollectedAmount == null);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.CollectedAmount = dto.CollectedAmount;
            handReceiptItem.CollectionDate = DateTime.Now;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int handReceiptItemId, int handReceiptId, string userId)
        {
            var handReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == handReceiptItemId && x.HandReceiptId == handReceiptId
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Delivered;
            handReceiptItem.DeliveryDate = DateTime.Now;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int handReceiptId, string userId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.ReceiptItems.Where(x => x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered))
                .SingleOrDefaultAsync(x => x.Id == handReceiptId
                && !x.ReceiptItems.All(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered));
            if (handReceipt == null)
                throw new EntityNotFoundException();

            foreach (var handReceiptItem in handReceipt.ReceiptItems)
            {
                handReceiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Delivered;
                handReceiptItem.DeliveryDate = DateTime.Now;
                handReceiptItem.UpdatedAt = DateTime.Now;
                handReceiptItem.UpdatedBy = userId;
                _db.ReceiptItems.Update(handReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAllItemsDelivered(int handReceiptId)
        {
            var returnHandReceipt = await _db.HandReceipts.Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsDelivered = returnHandReceipt.ReceiptItems.All(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered);
            return isAllItemsDelivered;
        }

    }
}
