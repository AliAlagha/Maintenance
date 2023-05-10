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

        public async Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, string userId)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.TechnicianId.Equals(userId))
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ReceiptItemForMaintenanceViewModel>(pagination, _mapper);
        }

        public async Task CompleteMaintenance(int receiptItemId, string userId)
        {
            var receiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == receiptItemId
                && x.TechnicianId.Equals(userId));
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Completed;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task CustomerRefuseMaintenance(CustomerRefuseMaintenanceDto dto, string userId)
        {
            var receiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.TechnicianId.Equals(userId));
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.CustomerRefused;
            receiptItem.ReasonForRefusingMaintenance = dto.ReasonForRefusingMaintenance;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task SuspenseMaintenance(SuspenseReceiptItemDto dto, string userId)
        {
            var receiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.TechnicianId.Equals(userId));
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.MaintenanceRequestStatus = MaintenanceRequestStatus.Suspended;
            receiptItem.MaintenanceSuspensionReason = dto.MaintenanceSuspensionReason;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task EnterMaintenanceCost(EnterMaintenanceCostDto dto, string userId)
        {
            var receiptItem = await _db.ReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.ReceiptItemId
                && x.TechnicianId.Equals(userId) && x.NotifyCustomerOfTheCost);
            if (receiptItem == null)
                throw new EntityNotFoundException();

            receiptItem.CostNotifiedToTheCustomer = dto.CostNotifiedToTheCustomer;
            receiptItem.UpdatedAt = DateTime.Now;
            receiptItem.UpdatedBy = userId;
            _db.ReceiptItems.Update(receiptItem);
            await _db.SaveChangesAsync();
        }

    }
}
