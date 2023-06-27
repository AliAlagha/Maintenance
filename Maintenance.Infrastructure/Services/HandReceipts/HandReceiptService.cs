using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System.Globalization;
using Maintenance.Infrastructure.Services.PdfExportReport;
using Maintenance.Infrastructure.Services.Barcodes;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharpCore.Drawing;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class HandReceiptService : IHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPdfExportReportService _pdfExportReportService;
        private readonly IBarcodeService _barcodeService;

        public HandReceiptService(ApplicationDbContext db, IMapper mapper
            , IPdfExportReportService pdfExportReportService
            , IBarcodeService barcodeService)
        {
            _db = db;
            _mapper = mapper;
            _pdfExportReportService = pdfExportReportService;
            _barcodeService = barcodeService;
        }

        public async Task<PagingResultViewModel<HandReceiptViewModel>> GetAll
            (Pagination pagination, QueryDto query
            , MaintenanceType maintenanceType, string? barcode)
        {
            var dbQuery = _db.HandReceipts
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItems)
                .Include(x => x.ReturnHandReceipt)
                .Where(x => x.MaintenanceType == maintenanceType)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.HandReceiptItems.Any(x => x.ItemBarcode.Contains(query.GeneralSearch)));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                dbQuery = dbQuery.Where(x => x.HandReceiptItems.Any(x => x.ItemBarcode.Contains(barcode)));
            }

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CustomerId == query.CustomerId);
            }

            return await dbQuery.ToPagedData<HandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<HandReceipt> Create(CreateHandReceiptDto input, MaintenanceType maintenanceType
            , string userId)
        {
            var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == userId);
            if (currentUser == null)
            {
                throw new EntityNotFoundException();
            }

            var customer = await _db.Customers.SingleOrDefaultAsync(x => x.Id == input.CustomerId);

            var handReceipt = _mapper.Map<HandReceipt>(input);
            handReceipt.BranchId = currentUser.BranchId;
            await AddHandReceiptItems(input.Items, handReceipt, customer, userId);

            handReceipt.MaintenanceType = maintenanceType;
            handReceipt.Date = DateTime.Now;
            handReceipt.CreatedBy = userId;
            await _db.HandReceipts.AddAsync(handReceipt);
            await _db.SaveChangesAsync();
            return handReceipt;
        }

        public async Task AddHandReceiptItems(List<CreateHandReceiptItemDto> itemDtos
            , HandReceipt handReceipt, Customer customer, string userId)
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
                var handReceiptItem = _mapper.Map<HandReceiptItem>(itemDto);

                handReceiptItem.HandReceiptId = handReceipt.Id;
                handReceiptItem.CustomerId = handReceipt.CustomerId;
                handReceiptItem.Item = dbItems.Single(x => x.Id == itemDto.ItemId).Name;
                handReceiptItem.Company = dbCompanies.Single(x => x.Id == itemDto.CompanyId).Name;
                if (itemDto.ColorId != null)
                {
                    handReceiptItem.Color = dbColors.Single(x => x.Id == itemDto.ColorId).Name;
                }
                handReceiptItem.ItemBarcode = await GenerateBarcode();

                if (itemDto.SpecifiedCost != null)
                {
                    handReceiptItem.FinalCost = itemDto.SpecifiedCost;
                }
                else if (itemDto.CostFrom != null || itemDto.CostTo != null)
                {
                    handReceiptItem.NotifyCustomerOfTheCost = true;
                }

                var customerName = "";
                if (customer != null)
                {
                    customerName = customer.Name;
                }
                else
                {
                    customerName = "صيانة فورية";
                }

                handReceiptItem.ItemBarcodeFilePath = _barcodeService.GenerateBarcode(handReceiptItem.ItemBarcode
                    , customerName);

                handReceiptItem.BranchId = handReceipt.BranchId;
                handReceiptItem.CreatedBy = userId;
                handReceipt.HandReceiptItems.Add(handReceiptItem);
            }
        }

        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(6);
            var isBarcodeExists = await _db.HandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
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
            var handReceipt = await _db.HandReceipts
                .SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            handReceipt.IsDelete = true;
            handReceipt.UpdatedAt = DateTime.Now;
            handReceipt.UpdatedBy = userId;
            _db.HandReceipts.Update(handReceipt);
            await _db.SaveChangesAsync();
        }

        public async Task<byte[]> ExportToPdf(int id)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItems)
                .Include(x => x.Branch)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("HandReceiptNumber", handReceipt.Id);
            paramaters.Add("Date", handReceipt.Date.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture));
            paramaters.Add("CustomerName", handReceipt.Customer != null ? handReceipt.Customer.Name : "");
            paramaters.Add("CustomerPhoneNumber", handReceipt.Customer != null ? handReceipt.Customer.PhoneNumber : "");

            var branchPhoneNumber = "";
            var branchAddress = "";
            if (handReceipt.Branch != null)
            {
                branchPhoneNumber = handReceipt.Branch.PhoneNumber;
				branchAddress = handReceipt.Branch.Address;
			}

			paramaters.Add("BranchPhoneNumber", branchPhoneNumber);
			paramaters.Add("BranchAddress", branchAddress);

			var receiptItems = new List<ReceiptItemDataSet>();
            foreach (var receiptItem in handReceipt.HandReceiptItems)
            {
				var priceStr = "";
				if (receiptItem.SpecifiedCost != null)
				{
					priceStr = receiptItem.SpecifiedCost.ToString();
                }
                else if (receiptItem.CostFrom != null && receiptItem.CostTo != null)
                {
					priceStr = $"{Messages.From} {receiptItem.CostFrom} {Messages.To} {receiptItem.CostTo}";
                }
                else if (receiptItem.NotifyCustomerOfTheCost)
                {
                    priceStr = $"{Messages.NotifyCustomerOfTheCostMsg}";
                }

				var receiptItemDataSet = new ReceiptItemDataSet
                {
                    Item = receiptItem.Item,
                    Company = receiptItem.Company,
                    Color = receiptItem.Color,
                    Price = priceStr,
                    Description = receiptItem.Description
                };

                receiptItems.Add(receiptItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemDataSet", Data = receiptItems } };
            var result = _pdfExportReportService.GeneratePdf("HandReceipt.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> ExportBarcodesToPdf(int id)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var document = new PdfDocument();

            var customerName = "";
            if (handReceipt.Customer != null)
            {
                customerName = handReceipt.Customer.Name;
            }

            foreach (var item in handReceipt.HandReceiptItems)
            {
                string htmlelement = "<div style='width:100%;' style='text-align: center;'>";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                    , "Images", item.ItemBarcodeFilePath);
                htmlelement += "<img height='50' src='" + filePath + "'   />";
                htmlelement += "</div>";
                PdfGenerator.AddPdfPages(document, htmlelement, new PdfGenerateConfig
                {
                    ManualPageSize = new XSize(80, 55)
                });
            }

            byte[] response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            return response;
        }

    }
}
