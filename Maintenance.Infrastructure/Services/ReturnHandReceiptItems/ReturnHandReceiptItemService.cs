﻿using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Data.Extensions;
using Maintenance.Core.Resources;
using Maintenance.Core.Enums;

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
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Technician)
                .Where(x => x.ReturnHandReceiptId == returnHandReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> ItemsPagedData(IQueryable<ReceiptItem> query
            , Pagination dto)
        {
            var pageSize = dto.PerPage;
            var skip = (int)Math.Ceiling(pageSize * (decimal)(dto.Page - 1));

            var totalCount = await query.CountAsync();
            query = query.Skip(skip).Take(pageSize);

            var returnHandReceiptItems = await query.ToListAsync();

            var returnHandReceiptItemVms = new List<ReturnHandReceiptItemViewModel>();
            foreach (var item in returnHandReceiptItems)
            {
                var itemVm = _mapper.Map<ReturnHandReceiptItemViewModel>(item);
                if (item.WarrantyExpiryDate != null)
                {
                    var isWarrantyValid = item.WarrantyExpiryDate >= DateTime.Now.Date;
                    var warrantyExpiryDate = item.WarrantyExpiryDate.Value.ToString("yyyy-MM-dd");
                    var isWarrantyValidStr = isWarrantyValid ? Messages.WarrantyValid : Messages.WarrantyExpired;
                    itemVm.WarrantyExpiryDate = $"{warrantyExpiryDate} ({isWarrantyValidStr})";
                }

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

                returnHandReceiptItemVms.Add(itemVm);
            }

            return new PagingResultViewModel<ReturnHandReceiptItemViewModel>
            {
                Meta = new MetaViewModel
                {
                    Page = dto.Page,
                    Perpage = dto.PerPage,
                    Total = totalCount
                },
                Data = returnHandReceiptItemVms
            };
        }

        public async Task Delete(int returnHandReceiptItemId, int returnHandReceiptId, string userId)
        {
            var returnHandReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptItemId
                && x.ReturnHandReceiptId == returnHandReceiptId);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.IsDelete = true;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int returnHandReceiptItemId, int returnHandReceiptId, string userId)
        {
            var returnHandReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptItemId
                && x.ReturnHandReceiptId == returnHandReceiptId 
                && x.MaintenanceRequestStatus != Core.Enums.MaintenanceRequestStatus.Delivered);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Delivered;
            returnHandReceiptItem.DeliveryDate = DateTime.Now;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int returnHandReceiptId, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.ReceiptItems.Where(x => x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered))
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId
                && !x.ReceiptItems.All(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered));
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            foreach (var returnHandReceiptItem in returnHandReceipt.ReceiptItems)
            {
                returnHandReceiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Delivered;
                returnHandReceiptItem.DeliveryDate = DateTime.Now;
                returnHandReceiptItem.UpdatedAt = DateTime.Now;
                returnHandReceiptItem.UpdatedBy = userId;
                _db.ReceiptItems.Update(returnHandReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAllItemsDelivered(int returnHandReceiptId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsDelivered = returnHandReceipt.ReceiptItems.All(x => x.MaintenanceRequestStatus 
            == MaintenanceRequestStatus.Delivered);
            return isAllItemsDelivered;
        }

    }
}