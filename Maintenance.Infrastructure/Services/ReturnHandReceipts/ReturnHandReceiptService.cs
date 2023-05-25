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
using Maintenance.Core.Enums;
using Maintenance.Core.Helpers;
using Maintenance.Core.Resources;

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
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.ReceiptItems.Any(x => x.ItemBarcode.Contains(query.GeneralSearch)));
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
            var isReturnReceiptExists = await _db.ReturnHandReceipts.AnyAsync(x => x.HandReceiptId 
                == input.HandReceiptId);
            if (isReturnReceiptExists)
            {
                throw new AlreadyExistsException();
            }

            var handReceipt = await _db.HandReceipts
                .Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == input.HandReceiptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var selectedReturnHandReceiptItems_dto = input.Items.Where(x => x.IsSelected)
                .DistinctBy(x => x.HandReceiptItemId).ToList();
            var selectedReturnHandReceiptItemIds = selectedReturnHandReceiptItems_dto
                .Select(x => x.HandReceiptItemId).ToList();

            var dbSelectedItems = handReceipt.ReceiptItems.Where(x => selectedReturnHandReceiptItemIds.Contains(x.Id))
                .ToList();
            if (dbSelectedItems.Any(x => x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered))
            {
                throw new InvalidInputException();
            }

            var returnHandReceipt = _mapper.Map<ReturnHandReceipt>(input);
            await AddReturnHandReceiptItems(selectedReturnHandReceiptItems_dto, handReceipt, returnHandReceipt);

            returnHandReceipt.CustomerId = handReceipt.CustomerId;
            returnHandReceipt.CreatedBy = userId;
            await _db.ReturnHandReceipts.AddAsync(returnHandReceipt);
            await _db.SaveChangesAsync();
            return returnHandReceipt.Id;
        }

        private async Task AddReturnHandReceiptItems(List<CreateReturnHandReceiptItemDto>
            selectedReturnHandReceiptItems_dto, HandReceipt? handReceipt, ReturnHandReceipt returnHandReceipt)
        {
            foreach (var returnHandReceiptItem in selectedReturnHandReceiptItems_dto)
            {
                var handReceiptItem = handReceipt.ReceiptItems
                    .Single(x => x.Id == returnHandReceiptItem.HandReceiptItemId);

                MaintenanceRequestStatus status;
                if (handReceiptItem.WarrantyDaysNumber != null)
                {
                    var warrantyExpiryDate = handReceiptItem.DeliveryDate.Value.AddDays(handReceiptItem.WarrantyDaysNumber.Value);
                    var isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                    status = isWarrantyValid ? MaintenanceRequestStatus.New : MaintenanceRequestStatus.WaitingManagerResponse;
                }
                else
                {
                    status = MaintenanceRequestStatus.WaitingManagerResponse;
                }

                var newReturnHandReceiptItem = new ReceiptItem
                {
                    CustomerId = handReceipt.CustomerId,
                    ReturnHandReceiptId = returnHandReceipt.Id,
                    Item = handReceiptItem.Item,
                    Color = handReceiptItem.Color,
                    Description = handReceiptItem.Description,
                    Company = handReceiptItem.Company,
                    ItemBarcode = await GenerateBarcode(),
                    ReturnReason = returnHandReceiptItem.ReturnReason,
                    ReceiptItemType = ReceiptItemType.Returned,
                    PreviousReceiptItemId = handReceiptItem.Id,
                    PreviousTechnicianId = handReceiptItem.TechnicianId,
                    MaintenanceRequestStatus = status
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
                .Include(x => x.ReturnHandReceipt)
                .ThenInclude(x => x.ReceiptItems)
                .Include(x => x.ReceiptItems.Where(x =>
                    x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered))
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            var handReceiptItemsForReturn = new List<ReceiptItem>();
            if (handReceipt.ReturnHandReceipt != null)
            {
                var alreadySelectedItems = handReceipt.ReturnHandReceipt.ReceiptItems.Select(x => x.PreviousReceiptItemId).ToList();
                var notSelectedItems = handReceipt.ReceiptItems.Where(x => !alreadySelectedItems.Contains(x.Id)).ToList();
                handReceiptItemsForReturn.AddRange(notSelectedItems);
            }
            else
            {
                handReceiptItemsForReturn.AddRange(handReceipt.ReceiptItems);
            }

            var itemVms = new List<HandReceiptItemForReturnViewModel>();

            for (int i = 0; i < handReceiptItemsForReturn.Count; i++)
            {
                var handReceiptItem = handReceiptItemsForReturn[i];
                var handReceiptItemForReturnVm = new HandReceiptItemForReturnViewModel
                {
                    Index = i,
                    Id = handReceiptItem.Id,
                    Item = handReceiptItem.Item,
                    ItemBarcode = handReceiptItem.ItemBarcode,
                    Company = handReceiptItem.Company,
                    DeliveryDate = handReceiptItem.DeliveryDate != null
                        ? handReceiptItem.DeliveryDate.Value.ToString("yyyy-MM-dd")
                        : null,
                };

                if (handReceiptItem.WarrantyDaysNumber != null)
                {
                    var warrantyExpiryDate = handReceiptItem.DeliveryDate.Value.AddDays(handReceiptItem.WarrantyDaysNumber.Value);
                    var isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                    var warrantyMsg = isWarrantyValid ? Messages.WarrantyValid : Messages.WarrantyExpired;
                    handReceiptItemForReturnVm.WarrantyDaysNumber = handReceiptItem.WarrantyDaysNumber + " - " + warrantyMsg;
                }

                itemVms.Add(handReceiptItemForReturnVm);
            }

            return itemVms;
        }

        public async Task IsReturnReceiptAlradyExists(int handReceiptId)
        {
            var isReturnReceiptExists = await _db.ReturnHandReceipts.AnyAsync(x => x.HandReceiptId
                == handReceiptId);
            if (isReturnReceiptExists)
            {
                throw new AlreadyExistsException();
            }
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
