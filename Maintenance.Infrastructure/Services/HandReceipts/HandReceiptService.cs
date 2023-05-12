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

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class HandReceiptService : IHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;

        public HandReceiptService(ApplicationDbContext db, IMapper mapper
            , ICustomerService customerService)
        {
            _db = db;
            _mapper = mapper;
            _customerService = customerService;
        }

        public async Task<PagingResultViewModel<HandReceiptViewModel>> GetAll
            (Pagination pagination, QueryDto query, string? barcode)
        {
            var dbQuery = _db.HandReceipts
                .Include(x => x.Customer)
                .Include(x => x.ReceiptItems)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.ReceiptItems.Any(x => x.ItemBarcode.Contains(query.GeneralSearch)));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                dbQuery = dbQuery.Where(x => x.ReceiptItems.Any(x => x.ItemBarcode.Contains(barcode)));
            }

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CustomerId == query.CustomerId);
            }

            return await dbQuery.ToPagedData<HandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<int?> Create(CreateHandReceiptDto input, string userId)
        {
            int? id = null;
            await _db.UseTransaction(async () =>
            {
                var handReceipt = _mapper.Map<HandReceipt>(input);

                if (input.CustomerId == null && input.CustomerInfo != null)
                {
                    var createCustomerDto = _mapper.Map<CreateCustomerForHandReceiptDto
                        , CreateCustomerDto>(input.CustomerInfo);
                    handReceipt.CustomerId = await _customerService.Create(createCustomerDto, userId);
                }

                await AddHandReceiptItems(input.Items, handReceipt, userId);

                handReceipt.Date = DateTime.Now;
                handReceipt.CreatedBy = userId;
                await _db.HandReceipts.AddAsync(handReceipt);
                await _db.SaveChangesAsync();
                id = handReceipt.Id;
            });

            return id;
        }

        public async Task AddHandReceiptItems(List<CreateHandReceiptItemDto> itemDtos
            ,HandReceipt handReceipt, string userId)
        {
            var dtoItemIds = itemDtos.Select(x => x.ItemId).ToList();
            var dbItems = await _db.Items.Where(x => dtoItemIds.Contains(x.Id))
                .ToListAsync();

            var dtoCompanyIds = itemDtos.Select(x => x.CompanyId).ToList();
            var dbCompanies = await _db.Companies.Where(x => dtoCompanyIds.Contains(x.Id))
                .ToListAsync();

            List<Color>? dbColors = null;
            var dtoColorIds = itemDtos.Select(x => x.ColorId).ToList();
            if (dtoColorIds.Any())
            {
                dbColors = await _db.Colors.Where(x => dtoColorIds.Contains(x.Id))
                .ToListAsync();
            }

            foreach (var itemDto in itemDtos)
            {
                var handReceiptItem = _mapper.Map<ReceiptItem>(itemDto);

                handReceiptItem.HandReceiptId = handReceipt.Id;
                handReceiptItem.CustomerId = handReceipt.CustomerId;
                handReceiptItem.Item = dbItems.Single(x => x.Id == itemDto.ItemId).Name;
                handReceiptItem.Company = dbCompanies.Single(x => x.Id == itemDto.CompanyId).Name;
                if (itemDto.ColorId != null)
                {
                    handReceiptItem.Color = dbColors.Single(x => x.Id == itemDto.ColorId).Name;
                }
                handReceiptItem.ItemBarcode = await GenerateBarcode();
                handReceiptItem.ReceiptItemType = ReceiptItemType.New;
                handReceiptItem.CreatedBy = userId;

                handReceipt.ReceiptItems.Add(handReceiptItem);
            }
        }

        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExists = await _db.ReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
            if (isBarcodeExists)
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

        public async Task Delete(int id, string userId)
        {
            var handReceipt = await _db.HandReceipts.SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            handReceipt.IsDelete = true;
            handReceipt.UpdatedAt = DateTime.Now;
            handReceipt.UpdatedBy = userId;
            _db.HandReceipts.Update(handReceipt);
            await _db.SaveChangesAsync();
        }

    }
}
