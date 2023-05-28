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

namespace Maintenance.Infrastructure.Services.RecipientMaintenances
{
    public class RecipientMaintenanceService : IRecipientMaintenanceService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public RecipientMaintenanceService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<RecipientMaintenanceViewModel>> GetAll
            (Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.RecipientMaintenances
                .Include(x => x.Technician)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.Technician.FullName.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<RecipientMaintenanceViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateRecipientMaintenanceDto input, string userId)
        {
            var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                throw new EntityNotFoundException();
            }

            var recipientMaintenance = _mapper.Map<RecipientMaintenance>(input);
            recipientMaintenance.BranchId = currentUser.BranchId;
            recipientMaintenance.CreatedBy = userId;
            await _db.RecipientMaintenances.AddAsync(recipientMaintenance);
            await _db.SaveChangesAsync();
            return recipientMaintenance.Id;
        }

        public async Task Delete(int id, string userId)
        {
            var recipientMaintenance = await _db.RecipientMaintenances.SingleOrDefaultAsync(x => x.Id == id);
            if (recipientMaintenance == null)
                throw new EntityNotFoundException();

            recipientMaintenance.IsDelete = true;
            recipientMaintenance.UpdatedAt = DateTime.Now;
            recipientMaintenance.UpdatedBy = userId;
            _db.RecipientMaintenances.Update(recipientMaintenance);
            await _db.SaveChangesAsync();
        }
    }
}
