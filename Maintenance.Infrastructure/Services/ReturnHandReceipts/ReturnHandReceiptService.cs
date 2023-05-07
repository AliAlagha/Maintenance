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

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class ReturnHandReceiptService : IReturnHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ReturnHandReceiptService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<PagingResultViewModel<ReturnHandReceiptViewModel>> GetAll(Pagination pagination
            , QueryDto query)
        {
            var dbQuery = _db.ReturnHandReceipts
                .Include(x => x.HandReceipt)
                .ThenInclude(x => x.Customer)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.HandReceipt.Customer.Name.Contains(query.GeneralSearch)
                    || x.HandReceipt.Customer.Email.Contains(query.GeneralSearch)
                    || x.HandReceipt.Customer.PhoneNumber.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ReturnHandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateReturnHandReceiptDto input, string userId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == input.HandReciptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var returnHandReceipt = _mapper.Map<ReturnHandReceipt>(input);
            await AddReturnHandReceiptItems(input, handReceipt, returnHandReceipt);

            returnHandReceipt.CreatedBy = userId;
            await _db.ReturnHandReceipts.AddAsync(returnHandReceipt);
            await _db.SaveChangesAsync();
            return returnHandReceipt.Id;
        }

        private async Task AddReturnHandReceiptItems(CreateReturnHandReceiptDto input, HandReceipt? handReceipt, ReturnHandReceipt returnHandReceipt)
        {
            foreach (var returnHandReceiptItem in input.ReturnHandReceiptItems)
            {
                var handReceiptItem = handReceipt.HandReceiptItems
                    .Single(x => x.Id == returnHandReceiptItem.Id);

                var newReturnHandReceiptItem = new ReturnHandReceiptItem
                {
                    ReturnHandReceiptId = returnHandReceipt.Id,
                    Item = handReceiptItem.Item,
                    Color = handReceiptItem.Color,
                    Description = handReceiptItem.Description,
                    Company = handReceiptItem.Company,
                    ItemBarcode = await GenerateBarcode(),
                    WarrantyExpiryDate = handReceiptItem.WarrantyExpiryDate,
                    ReturnReason = returnHandReceiptItem.ReturnReason
                };

                returnHandReceipt.ReturnHandReceiptItems.Add(newReturnHandReceiptItem);
            }
        }

        public async Task Delete(int id, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts.SingleOrDefaultAsync(x => x.Id == id);
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            returnHandReceipt.IsDelete = true;
            returnHandReceipt.UpdatedAt = DateTime.Now;
            returnHandReceipt.UpdatedBy = userId;
            _db.ReturnHandReceipts.Update(returnHandReceipt);
            await _db.SaveChangesAsync();
        }

        // Items
        public async Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, int returnHandReceiptId)
        {
            var dbQuery = _db.ReturnHandReceiptItems
                .Where(x => x.ReturnHandReceiptId == returnHandReceiptId)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Item.Contains(query.GeneralSearch)
                    || x.ItemBarcode.Contains(query.GeneralSearch));
            }

            return await dbQuery.ToPagedData<ReturnHandReceiptItemViewModel>(pagination, _mapper);
        }

        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExistsInHandReceipt = await _db.HandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExistsInHandReceipt)
            {
                await GenerateBarcode();
            }

            var isBarcodeExistsInReturnHandReceipt = await _db.ReturnHandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExistsInReturnHandReceipt)
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

        public async Task DeleteReturnHandReceiptItem(int id, string userId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems.SingleOrDefaultAsync(x => x.Id == id);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.IsDelete = true;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task ReturnHandReceiptItemDelivery(HandReceiptItemDeliveryDto dto, string userId)
        {
            var returnHandReceiptItem = await _db.ReturnHandReceiptItems
                .SingleOrDefaultAsync(x => x.Id == dto.Id && x.Delivered == false);
            if (returnHandReceiptItem == null)
                throw new EntityNotFoundException();

            returnHandReceiptItem.Delivered = true;
            returnHandReceiptItem.DeliveryDate = dto.DeliveryDate;
            returnHandReceiptItem.UpdatedAt = DateTime.Now;
            returnHandReceiptItem.UpdatedBy = userId;
            _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            await _db.SaveChangesAsync();
        }

        public async Task DeliveryOfAllItems(DeliveryOfAllItemsDto dto, string userId)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.ReturnHandReceiptItems.Where(x => !x.Delivered))
                .SingleOrDefaultAsync(x => x.Id == dto.Id
                && !x.ReturnHandReceiptItems.All(x => x.Delivered));
            if (returnHandReceipt == null)
                throw new EntityNotFoundException();

            foreach (var returnHandReceiptItem in returnHandReceipt.ReturnHandReceiptItems)
            {
                returnHandReceiptItem.Delivered = true;
                returnHandReceiptItem.DeliveryDate = dto.DeliveryDate;
                returnHandReceiptItem.UpdatedAt = DateTime.Now;
                returnHandReceiptItem.UpdatedBy = userId;
                _db.ReturnHandReceiptItems.Update(returnHandReceiptItem);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<CreateReturnHandReceiptDto> GetHandReceiptInfo(int id)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            var itemDtos = new List<CreateReturnHandReceiptItemDto>();
            
            for (int i = 0; i < handReceipt.HandReceiptItems.Count; i++)
            {
                var handReceiptItem = handReceipt.HandReceiptItems[i];
                itemDtos.Add(new CreateReturnHandReceiptItemDto
                {
                    Index = i,
                    Id = handReceiptItem.Id,
                    Item  = handReceiptItem.Item,
                    ItemBarcode = handReceiptItem.ItemBarcode,
                    Company = handReceiptItem.Company
                });
            }


            var dto = new CreateReturnHandReceiptDto
            {
                HandReciptId = handReceipt.Id,
                ReturnHandReceiptItems = itemDtos
            };

            return dto;
        }

    }
}
