using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Extensions;
using Maintenance.Infrastructure.Services.Items;
using Microsoft.EntityFrameworkCore;

namespace Maintenance.Infrastructure.Services.Items
{
    public class ItemService : IItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ItemService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ItemViewModel>> GetAll(Pagination pagination, QueryDto query)
        {
            var dbQuery = _db.Items.OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Name.Contains(query.GeneralSearch)
                    || x.Description.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ItemViewModel>(pagination, _mapper);
        }

        public async Task<UpdateItemDto> Get(int id)
        {
            var item = await _db.Items.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
                throw new EntityNotFoundException();

            return _mapper.Map<UpdateItemDto>(item);
        }

        public async Task<int> Create(CreateItemDto input, string userId)
        {
            var item = _mapper.Map<Item>(input);
            item.CreatedBy = userId;
            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();
            return item.Id;
        }

        public async Task Update(UpdateItemDto input, string userId)
        {
            var item = await _db.Items.SingleOrDefaultAsync(x => x.Id == input.Id);
            if (item == null)
                throw new EntityNotFoundException();

            _mapper.Map(input, item);

            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var item = await _db.Items.SingleOrDefaultAsync(x => x.Id == id);
            if (item == null)
                throw new EntityNotFoundException();

            item.IsDelete = true;
            item.UpdatedAt = DateTime.Now;
            item.UpdatedBy = userId;
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
        }

        public async Task<List<BaseVm>> List()
        {
            var items = _db.Items.AsQueryable();

            return await items.Select(x => new BaseVm
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
        }
    }
}
