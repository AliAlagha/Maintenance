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
using Maintenance.Infrastructure.Services.Barcodes;

namespace Maintenance.Infrastructure.Services.HandReceiptItems
{
    public class HandReceiptItemService : IHandReceiptItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IBarcodeService _barcodeService;

        public HandReceiptItemService(ApplicationDbContext db, IMapper mapper
            , IBarcodeService barcodeService)
        {
            _db = db;
            _mapper = mapper;
            _barcodeService = barcodeService;
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

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<HandReceiptItemViewModel>> ItemsPagedData(IQueryable<HandReceiptItem> query
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

                ShowWarrantyDays(item, itemVm);
                ShowRequestStatus(item, itemVm);

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

        private static void ShowWarrantyDays(HandReceiptItem? item, HandReceiptItemViewModel itemVm)
        {
            if (item.WarrantyDaysNumber != null)
            {
                if (item.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Delivered)
                {
                    var warrantyExpiryDate = item.DeliveryDate.Value.AddDays(item.WarrantyDaysNumber.Value);
                    var isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                    var warrantyMsg = isWarrantyValid ? Messages.WarrantyValid : Messages.WarrantyExpired;
                    itemVm.WarrantyDaysNumber = item.WarrantyDaysNumber + " - " + warrantyMsg;
                }
                else
                {
                    itemVm.WarrantyDaysNumber = item.WarrantyDaysNumber + " - " + Messages.WarrantyPeriodNotStarted;
                }
            }
        }

        private static void ShowRequestStatus(HandReceiptItem? item, HandReceiptItemViewModel itemVm)
        {
            switch (item.MaintenanceRequestStatus)
            {
                case HandReceiptItemRequestStatus.New:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                    break;
                case HandReceiptItemRequestStatus.CheckItem:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CheckItem}";
                    break;
                case HandReceiptItemRequestStatus.DefineMalfunction:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.DefineMalfunction}";
                    break;
                case HandReceiptItemRequestStatus.InformCustomerOfTheCost:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.InformCustomerOfTheCost}";
                    break;
                case HandReceiptItemRequestStatus.CustomerApproved:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerApproved}";
                    break;
                case HandReceiptItemRequestStatus.CustomerRefused:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerRefused} - {item.ReasonForRefusingMaintenance}";
                    break;
                case HandReceiptItemRequestStatus.NoResponseFromTheCustomer:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.NoResponseFromTheCustomer}";
                    break;
                case HandReceiptItemRequestStatus.ItemCannotBeServiced:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.ItemCannotBeServiced}";
                    break;
                case HandReceiptItemRequestStatus.NotifyCustomerOfTheInabilityToMaintain:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.NotifyCustomerOfTheInabilityToMaintain}";
                    break;
                case HandReceiptItemRequestStatus.EnterMaintenanceCost:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.EnterMaintenanceCost}";
                    break;
                case HandReceiptItemRequestStatus.Completed:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Completed}";
                    break;
                case HandReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.NotifyCustomerOfMaintenanceEnd}";
                    break;
                case HandReceiptItemRequestStatus.Delivered:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Delivered}";
                    break;
                case HandReceiptItemRequestStatus.Suspended:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                    break;
                case HandReceiptItemRequestStatus.RemovedFromMaintained:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.RemovedFromMaintained} - {item.RemoveFromMaintainedReason}";
                    break;
            };
        }

        public async Task<UpdateHandReceiptItemDto> Get(int handReceiptItemId, int handReceiptId)
        {
            var handReceiptItem = await _db.HandReceiptItems.SingleOrDefaultAsync(x => x.Id == handReceiptItemId
            && x.HandReceiptId == handReceiptId);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateHandReceiptItemDto>(handReceiptItem);
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

            if (input.SpecifiedCost != null)
            {
                handReceiptItem.FinalCost = input.SpecifiedCost;
            }
            else if (input.CostFrom != null || input.CostTo != null)
            {
                handReceiptItem.NotifyCustomerOfTheCost = true;
            }

            handReceiptItem.BranchId = handReceipt.BranchId;
            handReceiptItem.CustomerId = handReceipt.CustomerId;
            handReceiptItem.Item = item.Name;
            handReceiptItem.Company = company.Name;
            handReceiptItem.ItemBarcode = await GenerateBarcode();
            handReceiptItem.ItemBarcodeFilePath = _barcodeService.GenerateBarcode(handReceiptItem.ItemBarcode);
            handReceiptItem.CreatedBy = userId;
            await _db.HandReceiptItems.AddAsync(handReceiptItem);
            await _db.SaveChangesAsync();
            return handReceiptItem.Id;
        }

        //public async Task Update(UpdateHandReceiptItemDto input, string userId)
        //{
        //    var handReceiptItem = await _db.ReceiptItems.SingleOrDefaultAsync(x => x.Id == input.HandReceiptItemId
        //    && x.HandReceiptId == input.HandReceiptId);
        //    if (handReceiptItem == null)
        //    {
        //        throw new EntityNotFoundException();
        //    }

        //    if (input.TechnicianId != null)
        //    {
        //        var technician = await _db.Users.SingleOrDefaultAsync(x => x.Id.Equals(input.TechnicianId)
        //        && x.UserType == UserType.MaintenanceTechnician);
        //        if (technician == null)
        //        {
        //            throw new EntityNotFoundException();
        //        }
        //    }

        //    _mapper.Map(input, handReceiptItem);

        //    if (input.SpecifiedCost != null)
        //    {
        //        handReceiptItem.FinalCost = input.SpecifiedCost;
        //    }

        //    handReceiptItem.UpdatedAt = DateTime.Now;
        //    handReceiptItem.UpdatedBy = userId;
        //    _db.ReceiptItems.Update(handReceiptItem);
        //    await _db.SaveChangesAsync();
        //}

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
                && x.HandReceiptId == dto.HandReceiptId && x.FinalCost != null && x.CollectedAmount == null);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            var optionalDiscount = handReceiptItem.FinalCost * 0.1;
            var finalCostAfterOptionalDiscount = handReceiptItem.FinalCost - optionalDiscount;
            if (dto.CollectedAmount > handReceiptItem.FinalCost || dto.CollectedAmount < finalCostAfterOptionalDiscount)
            {
                throw new NotAllowedAmountException();
            }

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
                && x.TechnicianId != null
                && ((x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Delivered
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Suspended
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained)
                        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CustomerRefused
                        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.NoResponseFromTheCustomer
                        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.ItemCannotBeServiced
                        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.NotifyCustomerOfTheInabilityToMaintain));
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.Delivered;
            handReceiptItem.DeliveryDate = DateTime.Now;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int handReceiptId, string userId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.HandReceiptItems.Where(x =>
                    x.TechnicianId != null
                        && ((x.CollectedAmount != null
                        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Delivered
                        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Suspended
                        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained)
                            || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CustomerRefused
                            || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.NoResponseFromTheCustomer
                            || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.ItemCannotBeServiced
                            || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.NotifyCustomerOfTheInabilityToMaintain)))
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            foreach (var handReceiptItem in handReceipt.HandReceiptItems)
            {
                handReceiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.Delivered;
                handReceiptItem.DeliveryDate = DateTime.Now;
                handReceiptItem.UpdatedAt = DateTime.Now;
                handReceiptItem.UpdatedBy = userId;
                _db.HandReceiptItems.Update(handReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        //public async Task<bool> IsAllItemsCanBeDelivered(int handReceiptId)
        //{
        //    var returnHandReceipt = await _db.HandReceipts.Include(x => x.HandReceiptItems)
        //        .SingleOrDefaultAsync(x => x.Id == handReceiptId);
        //    if (returnHandReceipt == null)
        //    {
        //        throw new EntityNotFoundException();
        //    }

        //    var isAllItemsCanBeDelivered = false;
        //    if (returnHandReceipt.HandReceiptItems.Any())
        //    {
        //        isAllItemsCanBeDelivered = returnHandReceipt.HandReceiptItems.All(x =>
        //        x.TechnicianId != null
        //        && x.CollectedAmount != null
        //        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.CustomerRefused
        //        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.NoResponseFromTheCustomer
        //        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.ItemCannotBeServiced
        //        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.NotifyCustomerOfTheInabilityToMaintain
        //        && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Suspended
        //        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CustomerRefused
        //        || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.ItemCannotBeServiced);
        //    }

        //    return isAllItemsCanBeDelivered;
        //}

        public async Task RemoveFromMaintained(RemoveHandItemFromMaintainedDto dto, string userId)
        {
            var handReceiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.HandReceiptItemId && x.HandReceiptId == dto.HandReceiptId
                && x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Delivered);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.RemovedFromMaintained;
            handReceiptItem.RemoveFromMaintainedReason = dto.RemoveFromMaintainedReason;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task<HandReceipt> GetHandReceipt(int handReceiptId)
        {
            var handReceipt = await _db.HandReceipts.SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            return handReceipt;
        }
    }
}
