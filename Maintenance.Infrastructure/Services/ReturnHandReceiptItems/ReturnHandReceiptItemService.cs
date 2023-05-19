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

                switch (item.MaintenanceRequestStatus)
                {
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
                .ThenInclude(x => x.ReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == dto.ReturnHandReceiptId);
            if (returnHandReceipt == null || !returnHandReceipt.HandReceipt
                .ReceiptItems.Any(x => x.Id == dto.HandReceiptItemId))
            {
                throw new EntityNotFoundException();
            }

            var handReceiptItem = returnHandReceipt.HandReceipt.ReceiptItems
                .Single(x => x.Id == dto.HandReceiptItemId);

            if (handReceiptItem.MaintenanceRequestStatus != MaintenanceRequestStatus.Delivered)
            {
                throw new InvalidInputException();
            }

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
                CustomerId = returnHandReceipt.CustomerId,
                ReturnHandReceiptId = returnHandReceipt.Id,
                Item = handReceiptItem.Item,
                Color = handReceiptItem.Color,
                Description = handReceiptItem.Description,
                Company = handReceiptItem.Company,
                ItemBarcode = await GenerateBarcode(),
                ReturnReason = dto.ReturnReason,
                ReceiptItemType = ReceiptItemType.Returned,
                PreviousReceiptItemId = handReceiptItem.Id,
                PreviousTechnicianId = handReceiptItem.TechnicianId,
                MaintenanceRequestStatus = status,
            };

            newReturnHandReceiptItem.CreatedBy = userId;
            await _db.ReceiptItems.AddAsync(newReturnHandReceiptItem);
            await _db.SaveChangesAsync();
            return newReturnHandReceiptItem.Id;
        }

        public async Task Update(UpdateReturnHandReceiptItemDto dto, string userId)
        {
            var returnHandReceiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReturnHandReceiptItemId
                && x.ReturnHandReceiptId == dto.ReturnHandReceiptId);
            if (returnHandReceiptItem == null)
            {
                throw new EntityNotFoundException();
            }

            if (dto.TechnicianId != null)
            {
                var technician = await _db.Users.SingleOrDefaultAsync(x => x.Id.Equals(dto.TechnicianId)
                && x.UserType == UserType.MaintenanceTechnician);
                if (technician == null)
                {
                    throw new EntityNotFoundException();
                }
            }

            _mapper.Map(dto, returnHandReceiptItem);

            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
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
            var returnHandReceiptItem = await _db.ReceiptItems.SingleOrDefaultAsync(x => x.Id
                == returnHandReceiptItemId && x.ReturnHandReceiptId == returnHandReceiptId);
            if (returnHandReceiptItem == null)
            {
                throw new EntityNotFoundException();
            }

            return _mapper.Map<UpdateReturnHandReceiptItemDto>(returnHandReceiptItem);
        }
    }
}
