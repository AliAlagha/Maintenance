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

        private static void ShowWarrantyDays(ReceiptItem? item, HandReceiptItemViewModel itemVm)
        {
            if (item.WarrantyDaysNumber != null)
            {
                if (item.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered)
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

        private static void ShowRequestStatus(ReceiptItem? item, HandReceiptItemViewModel itemVm)
        {
            switch (item.MaintenanceRequestStatus)
            {
                case MaintenanceRequestStatus.WaitingManagerResponse:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.WaitingManagerResponse}";
                    break;
                case MaintenanceRequestStatus.ManagerApprovedReturn:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerApprovedReturn}";
                    break;
                case MaintenanceRequestStatus.ManagerRefusedReturn:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerRefusedReturn}";
                    break;
                case MaintenanceRequestStatus.New:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                    break;
                case MaintenanceRequestStatus.CheckItem:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CheckItem}";
                    break;
                case MaintenanceRequestStatus.InformCustomerOfTheCost:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.InformCustomerOfTheCost}";
                    break;
                case MaintenanceRequestStatus.CustomerApproved:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerApproved}";
                    break;
                case MaintenanceRequestStatus.EnterMaintenanceCost:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.EnterMaintenanceCost}";
                    break;
                case MaintenanceRequestStatus.Completed:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Completed}";
                    break;
                case MaintenanceRequestStatus.NotifyCustomerOfMaintenanceEnd:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.NotifyCustomerOfMaintenanceEnd}";
                    break;
                case MaintenanceRequestStatus.Delivered:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Delivered}";
                    break;
                case MaintenanceRequestStatus.CustomerRefused:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerRefused} - {item.ReasonForRefusingMaintenance}";
                    break;
                case MaintenanceRequestStatus.Suspended:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                    break;
                case MaintenanceRequestStatus.RemovedFromMaintained:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.RemovedFromMaintained} - {item.RemoveFromMaintainedReason}";
                    break;
            };
        }

        public async Task<UpdateHandReceiptItemDto> Get(int handReceiptItemId, int handReceiptId)
        {
            var handReceiptItem = await _db.ReceiptItems.SingleOrDefaultAsync(x => x.Id == handReceiptItemId
            && x.HandReceiptId == handReceiptId);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateHandReceiptItemDto>(handReceiptItem);
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

            if (input.SpecifiedCost != null)
            {
                handReceiptItem.FinalCost = input.SpecifiedCost;
            }
            else if (input.CostTo != null)
            {
                handReceiptItem.FinalCost = input.CostTo;
            }

            handReceiptItem.BranchId = handReceipt.BranchId;
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
            _db.ReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int handReceiptItemId, int handReceiptId, string userId)
        {
            var handReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == handReceiptItemId && x.HandReceiptId == handReceiptId
                && x.TechnicianId != null
                && x.CollectedAmount != null
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.WaitingManagerResponse
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.ManagerRefusedReturn
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.CustomerRefused
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Suspended
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.RemovedFromMaintained);
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
                .Include(x => x.ReceiptItems.Where(x =>
                    x.TechnicianId != null
                    && x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.WaitingManagerResponse
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.ManagerRefusedReturn
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.CustomerRefused
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Suspended
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.RemovedFromMaintained))
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

        public async Task<bool> IsAllItemsCanBeDelivered(int handReceiptId)
        {
            var returnHandReceipt = await _db.HandReceipts.Include(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsCanBeDelivered = false;
            if (returnHandReceipt.ReceiptItems.Any())
            {
                isAllItemsCanBeDelivered = returnHandReceipt.ReceiptItems.All(x =>
                x.TechnicianId != null
                && x.CollectedAmount != null
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.WaitingManagerResponse
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.ManagerRefusedReturn
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.CustomerRefused
                && x.MaintenanceRequestStatus != MaintenanceRequestStatus.Suspended);
            }

            return isAllItemsCanBeDelivered;
        }

        public async Task RemoveFromMaintained(RemoveHandItemFromMaintainedDto dto, string userId)
        {
            var handReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.HandReceiptItemId && x.HandReceiptId == dto.HandReceiptId
                && x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.RemovedFromMaintained;
            handReceiptItem.RemoveFromMaintainedReason = dto.RemoveFromMaintainedReason;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }

    }
}
