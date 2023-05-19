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

namespace Maintenance.Infrastructure.Services.ManagerRequests
{
    public class ManagerRequestService : IManagerRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ManagerRequestService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, string userId)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.WaitingManagerResponse)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch)
                    || x.HandReceiptId.ToString().Contains(query.GeneralSearch)
                    || x.ReturnHandReceiptId.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.HandReceipt.ReceiptItems.Any(x => x.ItemBarcode
                        .Contains(query.GeneralSearch))
                    || x.ReturnHandReceipt.ReceiptItems.Any(x => x.ItemBarcode
                        .Contains(query.GeneralSearch)));
            }

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> ItemsPagedData(IQueryable<ReceiptItem> query
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
                ShowRequestStatus(item, itemVm);

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

        private static void ShowRequestStatus(ReceiptItem? item, ReceiptItemForMaintenanceViewModel itemVm)
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

        public async Task UpdateStatus(int receiptItemId, MaintenanceRequestStatus status
            , string userId)
        {
            var receiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == receiptItemId);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.MaintenanceRequestStatus != MaintenanceRequestStatus.WaitingManagerResponse)
            {
                throw new NoValidityException();
            }

            if (status == MaintenanceRequestStatus.ManagerApprovedReturn)
            {
                receiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.ManagerApprovedReturn;
            }
            else if (status == MaintenanceRequestStatus.ManagerRefusedReturn)
            {
                receiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.ManagerRefusedReturn;
            }

            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }
    }
}
