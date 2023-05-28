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
using System.Globalization;
using Maintenance.Infrastructure.Services.PdfExportReport;
using Maintenance.Infrastructure.Services.Barcodes;

namespace Maintenance.Infrastructure.Services.InstantMaintenances
{
    public class InstantMaintenanceService : IInstantMaintenanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public InstantMaintenanceService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<InstantMaintenanceViewModel>> GetAll
            (Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.InstantMaintenances
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.InstantMaintenanceItems.Any(x => x.Item.Contains(query.GeneralSearch)));
            }

            return await dbQuery.ToPagedData<InstantMaintenanceViewModel>(pagination, _mapper);
        }

        public async Task<int?> Create(CreateInstantMaintenanceDto input, string userId)
        {
            var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                throw new EntityNotFoundException();
            }

            var instantMaintenance = _mapper.Map<InstantMaintenance>(input);
            instantMaintenance.BranchId = currentUser.BranchId;
            await AddInstantMaintenanceItems(input.Items, instantMaintenance, userId);

            instantMaintenance.Date = DateTime.Now;
            instantMaintenance.CreatedBy = userId;
            await _db.InstantMaintenances.AddAsync(instantMaintenance);
            await _db.SaveChangesAsync();
            return instantMaintenance.Id;
        }

        private async Task AddInstantMaintenanceItems(List<CreateInstantMaintenanceItemDto> itemDtos
            , InstantMaintenance instantMaintenance, string userId)
        {
            var dtoItemIds = itemDtos.Select(x => x.ItemId).ToList();
            var dbItems = await _db.Items.Where(x => dtoItemIds.Contains(x.Id))
                .ToListAsync();

            var dtoCompanyIds = itemDtos.Select(x => x.CompanyId).ToList();
            var dbCompanies = await _db.Companies.Where(x => dtoCompanyIds.Contains(x.Id))
                .ToListAsync();

            foreach (var itemDto in itemDtos)
            {
                var instantMaintenanceItem = _mapper.Map<InstantMaintenanceItem>(itemDto);

                instantMaintenanceItem.InstantMaintenanceId = instantMaintenance.Id;
                instantMaintenanceItem.Item = dbItems.Single(x => x.Id == itemDto.ItemId).Name;
                instantMaintenanceItem.Company = dbCompanies.Single(x => x.Id == itemDto.CompanyId).Name;
                instantMaintenanceItem.BranchId = instantMaintenance.BranchId;
                instantMaintenanceItem.CreatedBy = userId;
                instantMaintenance.InstantMaintenanceItems.Add(instantMaintenanceItem);
            }
        }

        public async Task Delete(int id, string userId)
        {
            var instantMaintenance = await _db.InstantMaintenances.SingleOrDefaultAsync(x => x.Id == id);
            if (instantMaintenance == null)
                throw new EntityNotFoundException();

            instantMaintenance.IsDelete = true;
            instantMaintenance.UpdatedAt = DateTime.Now;
            instantMaintenance.UpdatedBy = userId;
            _db.InstantMaintenances.Update(instantMaintenance);
            await _db.SaveChangesAsync();
        }
    }
}
