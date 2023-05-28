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
using DocumentFormat.OpenXml.VariantTypes;

namespace Maintenance.Infrastructure.Services.Maintenance
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public MaintenanceService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // Hand receipt items
        public async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllHandReceiptItems(Pagination pagination
            , QueryDto query, string userId)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch)
                    || x.HandReceiptId.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.HandReceipt.HandReceiptItems.Any(x => x.ItemBarcode
                        .Contains(query.GeneralSearch)));
            }

            return await HandReceiptItemsPagedData(handReceiptItemsDbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> HandReceiptItemsPagedData(IQueryable<HandReceiptItem> query
            , Pagination dto)
        {
            var pageSize = dto.PerPage;
            var skip = (int)Math.Ceiling(pageSize * (decimal)(dto.Page - 1));

            var totalCount = await query.CountAsync();
            query = query.Skip(skip).Take(pageSize);

            var items = await query.ToListAsync();

            var itemVms = new List<ReceiptItemForMaintenanceViewModel>();
            foreach (var item in items)
            {
                var itemVm = _mapper.Map<ReceiptItemForMaintenanceViewModel>(item);
                ShowRequestStatusForHandReceiptItem(item, itemVm);

                itemVms.Add(itemVm);
            }

            return new PagingResultViewModel<ReceiptItemForMaintenanceViewModel>
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

        private static void ShowRequestStatusForHandReceiptItem(HandReceiptItem? item, ReceiptItemForMaintenanceViewModel itemVm)
        {
            switch (item.MaintenanceRequestStatus)
            {
                case HandReceiptItemRequestStatus.New:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                    break;
                case HandReceiptItemRequestStatus.CheckItem:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CheckItem}";
                    break;
                case HandReceiptItemRequestStatus.InformCustomerOfTheCost:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.InformCustomerOfTheCost}";
                    break;
                case HandReceiptItemRequestStatus.CustomerApproved:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerApproved}";
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
                case HandReceiptItemRequestStatus.CustomerRefused:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CustomerRefused} - {item.ReasonForRefusingMaintenance}";
                    break;
                case HandReceiptItemRequestStatus.Suspended:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                    break;
                case HandReceiptItemRequestStatus.RemovedFromMaintained:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.RemovedFromMaintained} - {item.RemoveFromMaintainedReason}";
                    break;
            };
        }

        // Return hand receipt items
        public async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllReturnHandReceiptItems(Pagination pagination
            , QueryDto query, string userId)
        {
            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch)
                    || x.ReturnHandReceiptId.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.ReturnHandReceipt.ReturnHandReceiptItems.Any(x => x.ItemBarcode
                        .Contains(query.GeneralSearch)));
            }

            return await ReturnHandReceiptItemsPagedData(returnHandReceiptItemsDbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> ReturnHandReceiptItemsPagedData(IQueryable<ReturnHandReceiptItem> query
            , Pagination dto)
        {
            var pageSize = dto.PerPage;
            var skip = (int)Math.Ceiling(pageSize * (decimal)(dto.Page - 1));

            var totalCount = await query.CountAsync();
            query = query.Skip(skip).Take(pageSize);

            var items = await query.ToListAsync();

            var itemVms = new List<ReceiptItemForMaintenanceViewModel>();
            foreach (var item in items)
            {
                var itemVm = _mapper.Map<ReceiptItemForMaintenanceViewModel>(item);
                ShowRequestStatusForReturnHandReceiptItem(item, itemVm);

                itemVms.Add(itemVm);
            }

            return new PagingResultViewModel<ReceiptItemForMaintenanceViewModel>
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

        private static void ShowRequestStatusForReturnHandReceiptItem(ReturnHandReceiptItem? item, ReceiptItemForMaintenanceViewModel itemVm)
        {
            switch (item.MaintenanceRequestStatus)
            {
                case ReturnReceiptItemRequestStatus.WaitingManagerResponse:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.WaitingManagerResponse}";
                    break;
                case ReturnReceiptItemRequestStatus.ManagerApprovedReturn:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerApprovedReturn}";
                    break;
                case ReturnReceiptItemRequestStatus.ManagerRefusedReturn:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.ManagerRefusedReturn}";
                    break;
                case ReturnReceiptItemRequestStatus.New:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.New}";
                    break;
                case ReturnReceiptItemRequestStatus.CheckItem:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.CheckItem}";
                    break;
                case ReturnReceiptItemRequestStatus.Completed:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Completed}";
                    break;
                case ReturnReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.NotifyCustomerOfMaintenanceEnd}";
                    break;
                case ReturnReceiptItemRequestStatus.Delivered:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Delivered}";
                    break;
                case ReturnReceiptItemRequestStatus.Suspended:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.Suspended} - {item.MaintenanceSuspensionReason}";
                    break;
                case ReturnReceiptItemRequestStatus.RemovedFromMaintained:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.RemovedFromMaintained} - {item.RemoveFromMaintainedReason}";
                    break;
            };
        }

        // Hand receipt items
        public async Task UpdateStatusForHandReceipt(int receiptItemId, string userId)
        {
            var receiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == receiptItemId);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.TechnicianId != null && receiptItem.TechnicianId != userId)
            {
                throw new NoValidityException();
            }

            if (receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.New)
            {
                receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.CheckItem;
            }
            else if (receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CheckItem)
            {
                receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.InformCustomerOfTheCost;
            }
            else if (receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.InformCustomerOfTheCost)
            {
                receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.CustomerApproved;
            }
            else if ((receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CustomerApproved && !receiptItem.NotifyCustomerOfTheCost)
                || receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.EnterMaintenanceCost)
            {
                receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.Completed;
            }
            else if (receiptItem.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Completed)
            {
                receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd;
            }

            receiptItem.TechnicianId = userId;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task CustomerRefuseMaintenanceForHandReceipt(CustomerRefuseMaintenanceDto dto, string userId)
        {
            var receiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.TechnicianId != null && receiptItem.TechnicianId != userId)
            {
                throw new NoValidityException();
            }

            receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.CustomerRefused;
            receiptItem.ReasonForRefusingMaintenance = dto.ReasonForRefusingMaintenance;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task SuspenseMaintenanceForHandReceipt(SuspenseReceiptItemDto dto, string userId)
        {
            var receiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Suspended);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.Suspended;
            receiptItem.MaintenanceSuspensionReason = dto.MaintenanceSuspensionReason;
            receiptItem.TechnicianId = userId;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task EnterMaintenanceCostForHandReceipt(EnterMaintenanceCostDto dto, string userId)
        {
            var receiptItem = await _db.HandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.NotifyCustomerOfTheCost);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.TechnicianId != null && receiptItem.TechnicianId != userId)
            {
                throw new NoValidityException();
            }

            receiptItem.NotifyCustomerOfTheCost = false;
            receiptItem.CostNotifiedToTheCustomer = dto.CostNotifiedToTheCustomer;
            receiptItem.MaintenanceRequestStatus = HandReceiptItemRequestStatus.EnterMaintenanceCost;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.HandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        // Return hand receipt items
        public async Task UpdateStatusForReturnHandReceipt(int receiptItemId, string userId)
        {
            var receiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == receiptItemId);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.TechnicianId != null && receiptItem.TechnicianId != userId)
            {
                throw new NoValidityException();
            }

            if (receiptItem.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.ManagerApprovedReturn
                || receiptItem.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.New)
            {
                receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.CheckItem;
            }
            else if (receiptItem.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.CheckItem)
            {
                receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.Completed;
            }
            else if (receiptItem.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.Completed)
            {
                receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd;
            }

            receiptItem.TechnicianId = userId;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task SuspenseMaintenanceForReturnHandReceipt(SuspenseReceiptItemDto dto, string userId)
        {
            var receiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.MaintenanceRequestStatus != ReturnReceiptItemRequestStatus.Suspended);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.Suspended;
            receiptItem.MaintenanceSuspensionReason = dto.MaintenanceSuspensionReason;
            receiptItem.TechnicianId = userId;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }
    }
}
