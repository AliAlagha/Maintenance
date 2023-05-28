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
using Maintenance.Core.Helpers;
using Maintenance.Core.Resources;
using System.Globalization;
using Maintenance.Infrastructure.Services.PdfExportReport;
using Maintenance.Infrastructure.Services.Barcodes;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public class ReturnHandReceiptService : IReturnHandReceiptService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPdfExportReportService _pdfExportReportService;
        private readonly IBarcodeService _barcodeService;

        public ReturnHandReceiptService(ApplicationDbContext db, IMapper mapper
            , IPdfExportReportService pdfExportReportService
            , IBarcodeService barcodeService)
        {
            _db = db;
            _mapper = mapper;
            _pdfExportReportService = pdfExportReportService;
            _barcodeService = barcodeService;
        }

        public async Task<PagingResultViewModel<ReturnHandReceiptViewModel>> GetAll(Pagination pagination
            , QueryDto query, string? barcode)
        {
            var dbQuery = _db.ReturnHandReceipts
                .Include(x => x.HandReceipt)
                .ThenInclude(x => x.Customer)
                .Include(x => x.ReturnHandReceiptItems)
                .OrderByDescending(x => x.CreatedAt).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.GeneralSearch))
            {
                dbQuery = dbQuery.Where(x => x.Id.ToString().Contains(query.GeneralSearch)
                    || x.Customer.Name.Contains(query.GeneralSearch)
                    || x.Customer.PhoneNumber.Contains(query.GeneralSearch)
                    || x.ReturnHandReceiptItems.Any(x => x.ItemBarcode.Contains(query.GeneralSearch)));
            }

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                dbQuery = dbQuery.Where(x => x.ReturnHandReceiptItems.Any(x => x.ItemBarcode.Contains(barcode)));
            }

            if (query.CustomerId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.HandReceipt.Customer.Id == query.CustomerId);
            }

            return await dbQuery.ToPagedData<ReturnHandReceiptViewModel>(pagination, _mapper);
        }

        public async Task<int> Create(CreateReturnHandReceiptDto input, string userId)
        {
            var isReturnReceiptExists = await _db.ReturnHandReceipts.AnyAsync(x => x.HandReceiptId
                == input.HandReceiptId);
            if (isReturnReceiptExists)
            {
                throw new AlreadyExistsException();
            }

            var handReceipt = await _db.HandReceipts
                .Include(x => x.HandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == input.HandReceiptId);
            if (handReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var selectedReturnHandReceiptItems_dto = input.Items.Where(x => x.IsSelected)
                .DistinctBy(x => x.HandReceiptItemId).ToList();
            var selectedReturnHandReceiptItemIds = selectedReturnHandReceiptItems_dto
                .Select(x => x.HandReceiptItemId).ToList();

            var dbSelectedItems = handReceipt.HandReceiptItems.Where(x => selectedReturnHandReceiptItemIds.Contains(x.Id))
                .ToList();
            if (dbSelectedItems.Any(x => x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.Delivered))
            {
                throw new InvalidInputException();
            }

            var returnHandReceipt = _mapper.Map<ReturnHandReceipt>(input);
            returnHandReceipt.BranchId = handReceipt.BranchId;
            await AddReturnHandReceiptItems(selectedReturnHandReceiptItems_dto, handReceipt, returnHandReceipt);

            returnHandReceipt.CustomerId = handReceipt.CustomerId;
            returnHandReceipt.CreatedBy = userId;
            await _db.ReturnHandReceipts.AddAsync(returnHandReceipt);
            await _db.SaveChangesAsync();
            return returnHandReceipt.Id;
        }

        private async Task AddReturnHandReceiptItems(List<CreateReturnHandReceiptItemDto>
            selectedReturnHandReceiptItems_dto, HandReceipt? handReceipt, ReturnHandReceipt returnHandReceipt)
        {
            foreach (var returnHandReceiptItem in selectedReturnHandReceiptItems_dto)
            {
                var handReceiptItem = handReceipt.HandReceiptItems
                    .Single(x => x.Id == returnHandReceiptItem.HandReceiptItemId);

                ReturnReceiptItemRequestStatus status;
                bool isWarrantyValid = false;
                if (handReceiptItem.WarrantyDaysNumber != null)
                {
                    var warrantyExpiryDate = handReceiptItem.DeliveryDate.Value.AddDays(handReceiptItem.WarrantyDaysNumber.Value);
                    isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                    status = isWarrantyValid ? ReturnReceiptItemRequestStatus.New : ReturnReceiptItemRequestStatus.WaitingManagerResponse;
                }
                else
                {
                    status = ReturnReceiptItemRequestStatus.WaitingManagerResponse;
                }

                var newReturnHandReceiptItem = new ReturnHandReceiptItem
                {
                    CustomerId = handReceipt.CustomerId,
                    ReturnHandReceiptId = returnHandReceipt.Id,
                    Item = handReceiptItem.Item,
                    Color = handReceiptItem.Color,
                    Description = handReceiptItem.Description,
                    Company = handReceiptItem.Company,
                    ItemBarcode = await GenerateBarcode(),
                    ReturnReason = returnHandReceiptItem.ReturnReason,
                    HandReceiptItemId = handReceiptItem.Id,
                    MaintenanceRequestStatus = status,
                    BranchId = handReceiptItem.BranchId,
                    IsReturnItemWarrantyValid = isWarrantyValid 
                };

                newReturnHandReceiptItem.ItemBarcodeFilePath = _barcodeService.GenerateBarcode(newReturnHandReceiptItem.ItemBarcode);

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

        public async Task<List<HandReceiptItemForReturnViewModel>> GetHandReceiptItemsForReturn(int handReceiptId)
        {
            var handReceipt = await _db.HandReceipts
                .Include(x => x.ReturnHandReceipt)
                .ThenInclude(x => x.ReturnHandReceiptItems)
                .Include(x => x.HandReceiptItems.Where(x =>
                    x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Delivered))
                .SingleOrDefaultAsync(x => x.Id == handReceiptId);
            if (handReceipt == null)
                throw new EntityNotFoundException();

            var handReceiptItemsForReturn = new List<HandReceiptItem>();
            if (handReceipt.ReturnHandReceipt != null)
            {
                var alreadySelectedItems = handReceipt.ReturnHandReceipt.ReturnHandReceiptItems.Select(x => x.HandReceiptItemId).ToList();
                var notSelectedItems = handReceipt.HandReceiptItems.Where(x => !alreadySelectedItems.Contains(x.Id)).ToList();
                handReceiptItemsForReturn.AddRange(notSelectedItems);
            }
            else
            {
                handReceiptItemsForReturn.AddRange(handReceipt.HandReceiptItems);
            }

            var itemVms = new List<HandReceiptItemForReturnViewModel>();

            for (int i = 0; i < handReceiptItemsForReturn.Count; i++)
            {
                var handReceiptItem = handReceiptItemsForReturn[i];
                var handReceiptItemForReturnVm = new HandReceiptItemForReturnViewModel
                {
                    Index = i,
                    Id = handReceiptItem.Id,
                    Item = handReceiptItem.Item,
                    ItemBarcode = handReceiptItem.ItemBarcode,
                    Company = handReceiptItem.Company,
                    DeliveryDate = handReceiptItem.DeliveryDate != null
                        ? handReceiptItem.DeliveryDate.Value.ToString("yyyy-MM-dd")
                        : null,
                };

                if (handReceiptItem.WarrantyDaysNumber != null)
                {
                    var warrantyExpiryDate = handReceiptItem.DeliveryDate.Value.AddDays(handReceiptItem.WarrantyDaysNumber.Value);
                    var isWarrantyValid = DateTime.Now.Date <= warrantyExpiryDate.Date;
                    var warrantyMsg = isWarrantyValid ? Messages.WarrantyValid : Messages.WarrantyExpired;
                    handReceiptItemForReturnVm.WarrantyDaysNumber = handReceiptItem.WarrantyDaysNumber + " - " + warrantyMsg;
                }

                itemVms.Add(handReceiptItemForReturnVm);
            }

            return itemVms;
        }

        public async Task IsReturnReceiptAlradyExists(int handReceiptId)
        {
            var isReturnReceiptExists = await _db.ReturnHandReceipts.AnyAsync(x => x.HandReceiptId
                == handReceiptId);
            if (isReturnReceiptExists)
            {
                throw new AlreadyExistsException();
            }
        }

        public async Task<byte[]> ExportToPdf(int id)
        {
            var returnHandReceipt = await _db.ReturnHandReceipts
                .Include(x => x.Customer)
                .Include(x => x.ReturnHandReceiptItems)
                .SingleOrDefaultAsync(x => x.Id == id);
            if (returnHandReceipt == null)
            {
                throw new EntityNotFoundException();
            }

            var isAllItemsDelivered = returnHandReceipt.ReturnHandReceiptItems
                .All(x => x.MaintenanceRequestStatus == ReturnReceiptItemRequestStatus.Delivered);
            if (!isAllItemsDelivered)
            {
                throw new EntityNotFoundException();
            }

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReturnHandReceiptNumber", returnHandReceipt.Id);
            paramaters.Add("Date", returnHandReceipt.Date.ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture));
            paramaters.Add("CustomerName", returnHandReceipt.Customer.Name);
            paramaters.Add("CustomerPhoneNumber", returnHandReceipt.Customer.PhoneNumber);

            var isAnyReturnItemWarrantyExpired = returnHandReceipt.ReturnHandReceiptItems
                .Any(x => !x.IsReturnItemWarrantyValid);
            if (isAnyReturnItemWarrantyExpired)
            {
                paramaters.Add("Notes", Messages.WarrantyExpiredNote);
            }

            paramaters.Add("ContactEmail", "test@gmail.com");
            paramaters.Add("ContactPhoneNumber", "0599854758");
            paramaters.Add("WebsiteLink", "www.test.com");

            var receiptItems = new List<ReceiptItemDataSet>();
            foreach (var receiptItem in returnHandReceipt.ReturnHandReceiptItems)
            {
                var receiptItemDataSet = new ReceiptItemDataSet
                {
                    Item = receiptItem.Item,
                    ItemBarcode = receiptItem.ItemBarcode,
                    Company = receiptItem.Company
                };

                receiptItems.Add(receiptItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemDataSet", Data = receiptItems } };
            var result = _pdfExportReportService.GeneratePdf("ReturnHandReceipt.rdlc", dataSets, paramaters);
            return result;
        }

        // Heplers
        private async Task<string> GenerateBarcode()
        {
            var barcode = RandomDigits(10);
            var isBarcodeExists = await _db.ReturnHandReceiptItems.AnyAsync(x => x.ItemBarcode.Equals(barcode));
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

    }
}
