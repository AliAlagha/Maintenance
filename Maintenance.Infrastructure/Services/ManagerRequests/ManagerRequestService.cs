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
            var dbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.WaitingManagerResponse)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch)
                    || x.ReturnHandReceiptId.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.ReturnHandReceipt.ReturnHandReceiptItems.Any(x => x.ItemBarcode
                        .Contains(query.GeneralSearch)));
            }

            return await ItemsPagedData(dbQuery, pagination);
        }

        private async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> ItemsPagedData(IQueryable<ReturnHandReceiptItem> query
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

        private static void ShowRequestStatus(ReturnHandReceiptItem? item, ReceiptItemForMaintenanceViewModel itemVm)
        {
            switch (item.MaintenanceRequestStatus)
            {
                case ReturnReceiptItemRequestStatus.WaitingManagerResponse:
                    itemVm.MaintenanceRequestStatusMessage = $"{Messages.WaitingManagerResponse}";
                    break;
            };
        }

        public async Task UpdateStatus(int receiptItemId, ReturnReceiptItemRequestStatus status
            , string userId)
        {
            var receiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == receiptItemId);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            if (receiptItem.MaintenanceRequestStatus != ReturnReceiptItemRequestStatus.WaitingManagerResponse)
            {
                throw new NoValidityException();
            }

            if (status == ReturnReceiptItemRequestStatus.ManagerApprovedReturn)
            {
                receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.ManagerApprovedReturn;
            }
            else if (status == ReturnReceiptItemRequestStatus.ManagerRefusedReturn)
            {
                receiptItem.MaintenanceRequestStatus = ReturnReceiptItemRequestStatus.ManagerRefusedReturn;
            }

            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }
    }
}
