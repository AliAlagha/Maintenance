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

namespace Maintenance.Infrastructure.Services.InstantMaintenanceItems
{
    public class InstantMaintenanceItemService : IInstantMaintenanceItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IBarcodeService _barcodeService;

        public InstantMaintenanceItemService(ApplicationDbContext db, IMapper mapper
            , IBarcodeService barcodeService)
        {
            _db = db;
            _mapper = mapper;
            _barcodeService = barcodeService;
        }

        public async Task<PagingResultViewModel<InstantMaintenanceItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int instantMaintenanceId)
        {
            var dbQuery = _db.InstantMaintenanceItems
                .Include(x => x.Technician)
                .Where(x => x.InstantMaintenanceId == instantMaintenanceId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.Item.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<InstantMaintenanceItemViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateItemForExistsInstantMaintenanceDto input, string userId)
        {
            var instantMaintenanceItem = _mapper.Map<InstantMaintenanceItem>(input);

            var instantMaintenance = await _db.InstantMaintenances.SingleOrDefaultAsync(x => x.Id == input.InstantMaintenanceId);
            if (instantMaintenance == null)
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

            instantMaintenanceItem.BranchId = instantMaintenance.BranchId;
            instantMaintenanceItem.Item = item.Name;
            instantMaintenanceItem.Company = company.Name;
            instantMaintenanceItem.CreatedBy = userId;
            await _db.InstantMaintenanceItems.AddAsync(instantMaintenanceItem);
            await _db.SaveChangesAsync();
            return instantMaintenanceItem.Id;
        }

        public async Task Delete(int id, string userId)
        {
            var instantMaintenanceItem = await _db.InstantMaintenanceItems
                .SingleOrDefaultAsync(x => x.Id == id);
            if (instantMaintenanceItem == null)
                throw new EntityNotFoundException();

            instantMaintenanceItem.IsDelete = true;
            instantMaintenanceItem.UpdatedAt = DateTime.Now;
            instantMaintenanceItem.UpdatedBy = userId;
            _db.InstantMaintenanceItems.Update(instantMaintenanceItem);
            await _db.SaveChangesAsync();
        }

    }
}
