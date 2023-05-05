using AutoMapper;
using Maintenance.Core.Constants;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Data.Extensions;
using Maintenance.Infrastructure.Extensions;
using Maintenance.Infrastructure.Services.Files;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Infrastructure.Services.Colors
{
    public class ColorService : IColorService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ColorService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ColorViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Colors.OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.GeneralSearch)
                    || x.Description.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ColorViewModel>(pagination, _mapper);
        }

        public async Task<UpdateColorDto> Get(int id)
        {
            var color = await _db.Colors.SingleOrDefaultAsync(x => x.Id == id);
            if (color == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateColorDto>(color);
        }

        public async Task<int> Create(CreateColorDto input, string userId)
        {
            var color = _mapper.Map<Color>(input);
            color.CreatedBy = userId;
            await _db.Colors.AddAsync(color);
            await _db.SaveChangesAsync();
            return color.Id;
        }

        public async Task Update(UpdateColorDto input, string userId)
        {
            var color = await _db.Colors.SingleOrDefaultAsync(x => x.Id == input.Id);
            if (color == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, color);

            color.UpdatedAt = DateTime.Now;
            color.UpdatedBy = userId;
            _db.Colors.Update(color);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var color = await _db.Colors.SingleOrDefaultAsync(x => x.Id == id);
            if (color == null)
                throw new EntityNotFoundException();

            color.IsDelete = true;
            color.UpdatedAt = DateTime.Now;
            color.UpdatedBy = userId;
            _db.Colors.Update(color);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BaseVm>> List()
        {
            var colors = _db.Colors.AsQueryable();

            return await colors.Select(x => new BaseVm
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
