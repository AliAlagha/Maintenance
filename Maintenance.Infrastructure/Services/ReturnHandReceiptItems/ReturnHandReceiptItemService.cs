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
using Maintenance.Core.Resources;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Barcodes;

namespace Maintenance.Infrastructure.Services.ReturnHandReceiptItems
{
    public class ReturnHandReceiptItemService : IReturnHandReceiptItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IBarcodeService _barcodeService;

        public ReturnHandReceiptItemService(ApplicationDbContext db, IMapper mapper
            , IBarcodeService barcodeService)
        {
            _db = db;
            _mapper = mapper;
            _barcodeService = barcodeService;
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

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> ItemsPagedData(IQueryable<ReturnHandReceiptItem> query
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

                switch (item.MaintenanceRequestStatus)
                {
                    case ReturnHandReceiptItemRequestStatus.WaitingManagerResponse:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.WaitingManagerResponse}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.ManagerApprovedReturn:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerApprovedReturn}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerRefusedReturn}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.New:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.CheckItem:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.CheckItem}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.Completed:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Completed}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.NotifyCustomerOfMaintenanceEnd}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.Delivered:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Delivered}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.Suspended:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                        break;
                    case ReturnHandReceiptItemRequestStatus.RemovedFromMaintained:
                        itemVm.MaintenanceRequestStatusMessage = $"{Messages.RemovedFromMaintained} - {item.RemoveFromMaintainedReason}";
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

        public async Task<int> Create(CreateReturnItemForExistsReturnHandReceiptDto dto, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.HandReceipt)
                .ThenInclude(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == dto.ReturnHandReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isReceiptItemExistsInHandReceipt = returnHandReceipt.HandReceipt
                .HandReceiptItems.Any(x => x.Id == dto.HandReceiptItemId);
            if (!isReceiptItemExistsInHandReceipt)
            {
                throw new EntityNotFoundException();
            }

            var isReceiptItemAlreadySelected = returnHandReceipt.ReturnHandReceiptItems.Any(x => x.Id
                == dto.HandReceiptItemId);
            if (isReceiptItemAlreadySelected)
            {
                throw new AlreadyExistsException();
            }

            var handReceiptItem = returnHandReceipt.HandReceipt.HandReceiptItems
                .Single(x => x.Id == dto.HandReceiptItemId);

            if (handReceiptItem.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Delivered)
            {
                throw new InvalidInputException();
            }

            ReturnHandReceiptItemRequestStatus status;
            bool isWarrantyValid = false;
            if (handReceiptItem.WarrantyDaysNumber != null)
            {
                var warrantyExpiryDate = handReceiptItem.DeliveryDate.Value.AddDays(handReceiptItem.WarrantyDaysNumber.Value);
                isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                status = isWarrantyValid ? ReturnHandReceiptItemRequestStatus.New : ReturnHandReceiptItemRequestStatus.WaitingManagerResponse;
            }
            else
            {
                status = ReturnHandReceiptItemRequestStatus.WaitingManagerResponse;
            }

            var newReturnHandReceiptItem = new ReturnHandReceiptItem
            {
                CustomerId = returnHandReceipt.CustomerId,
                ReturnHandReceiptId = returnHandReceipt.Id,
                Item = handReceiptItem.Item,
                Color = handReceiptItem.Color,
                Description = handReceiptItem.Description,
                Company = handReceiptItem.Company,
                ItemBarcode = await GenerateBarcode(),
                ReturnReason = dto.ReturnReason,
                HandReceiptItemId = handReceiptItem.Id,
                MaintenanceRequestStatus = status,
                BranchId = returnHandReceipt.BranchId,
                IsReturnItemWarrantyValid = isWarrantyValid 
            };

            newReturnHandReceiptItem.ItemBarcodeFilePath = _barcodeService.GenerateBarcode(newReturnHandReceiptItem.ItemBarcode);

            newReturnHandReceiptItem.CreatedBy = userId;
            await _db.ReturnHandReceiptItems.AddAsync(newReturnHandReceiptItem);
            await _db.SaveChangesAsync();
            return newReturnHandReceiptItem.Id;
        }

        //public async Task Update(UpdateReturnHandReceiptItemDto dto, string userId)
        //{
        //    var returnHandReceiptItem = await _db.ReceiptItems
        //        .SingleOrDefaultAsync(x => x.Id == dto.ReturnHandReceiptItemId
        //        && x.ReturnHandReceiptId == dto.ReturnHandReceiptId);
        //    if (returnHandReceiptItem == null)
        //    {
        //        throw new EntityNotFoundException();
        //    }

        //    if (dto.TechnicianId != null)
        //    {
        //        var technician = await _db.Users.SingleOrDefaultAsync(x => x.Id.Equals(dto.TechnicianId)
        //        && x.UserType == UserType.MaintenanceTechnician);
        //        if (technician == null)
        //        {
        //            throw new EntityNotFoundException();
        //        }
        //    }

        //    _mapper.Map(dto, returnHandReceiptItem);

        //    returnHandReceiptItem.UpdatedAt = DateTime.Now;
        //    returnHandReceiptItem.UpdatedBy = userId;
        //    _db.ReceiptItems.Update(returnHandReceiptItem);
        //    await _db.SaveChangesAsync();
        //}

        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExists = await _db.ReturnHandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
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
            _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliverItem(int returnHandReceiptItemId, int returnHandReceiptId, string userId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptItemId
                && x.ReturnHandReceiptId == returnHandReceiptId
                && x.TechnicianId != null
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.WaitingManagerResponse
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.Delivered
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.Suspended
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.RemovedFromMaintained);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.MaintenanceRequestStatus = ReturnHandReceiptItemRequestStatus.Delivered;
            returnHandReceiptItem.DeliveryDate = DateTime.Now;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(int returnHandReceiptId, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.ReturnHandReceiptItems.Where(x =>
                    x.TechnicianId != null
                    && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.WaitingManagerResponse
                    && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn
                    && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.Delivered
                    && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.Suspended
                    && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.RemovedFromMaintained))
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId
                && !x.ReturnHandReceiptItems.All(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Delivered));
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            foreach (var returnHandReceiptItem in returnHandReceipt.ReturnHandReceiptItems)
            {
                returnHandReceiptItem.MaintenanceRequestStatus = ReturnHandReceiptItemRequestStatus.Delivered;
                returnHandReceiptItem.DeliveryDate = DateTime.Now;
                returnHandReceiptItem.UpdatedAt = DateTime.Now;
                returnHandReceiptItem.UpdatedBy = userId;
                _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> IsAllItemsCanBeDelivered(int returnHandReceiptId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.Include(x => x.ReturnHandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == returnHandReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsCanBeDelivered = false;
            if (returnHandReceipt.ReturnHandReceiptItems.Any())
            {
                isAllItemsCanBeDelivered = returnHandReceipt.ReturnHandReceiptItems.All(x =>
                x.TechnicianId != null
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.WaitingManagerResponse
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn
                && x.MaintenanceRequestStatus != ReturnHandReceiptItemRequestStatus.Suspended);
            }

            return isAllItemsCanBeDelivered;
        }

        public async Task<int> GetHandReceiptId(int returnHandReceiptId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.SingleOrDefaultAsync(x => x.Id == returnHandReceiptId);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            return returnHandReceipt.HandReceiptId;
        }

        public async Task<UpdateReturnHandReceiptItemDto> Get(int returnHandReceiptItemId, int returnHandReceiptId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems.SingleOrDefaultAsync(x => x.Id
                == returnHandReceiptItemId && x.ReturnHandReceiptId == returnHandReceiptId);
            if (returnHandReceiptItem == null)
            {
                throw new EntityNotFoundException();
            }

            return _mapper.Map<UpdateReturnHandReceiptItemDto>(returnHandReceiptItem);
        }

        public async Task RemoveFromMaintained(RemoveReturnItemFromMaintainedDto dto, string userId)
        {
            var handReceiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReturnHandReceiptItemId && x.ReturnHandReceiptId == dto.ReturnHandReceiptId
                && x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Delivered);
            if (handReceiptItem == null)
                throw new EntityNotFoundException();

            handReceiptItem.MaintenanceRequestStatus = ReturnHandReceiptItemRequestStatus.RemovedFromMaintained;
            handReceiptItem.RemoveFromMaintainedReason = dto.RemoveFromMaintainedReason;
            handReceiptItem.UpdatedAt = DateTime.Now;
            handReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(handReceiptItem);
            await _db.SaveChangesAsync();
        }
    }
}
