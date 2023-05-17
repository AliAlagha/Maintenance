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
using Maintenance.Infrastructure.Services.HandReceipts;

namespace Maintenance.Infrastructure.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPdfExportReportService _pdfExportReportService;

        public ReportService(ApplicationDbContext db, IMapper mapper
            , IPdfExportReportService pdfExportReportService)
        {
            _db = db;
            _mapper = mapper;
            _pdfExportReportService = pdfExportReportService;
        }

        public async Task<byte[]> ReceiptItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var receiptItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            paramaters.Add("ContactEmail", "test@gmail.com");
            paramaters.Add("ContactPhoneNumber", "0599854758");
            paramaters.Add("WebsiteLink", "www.test.com");

            var receiptItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var receiptItem in receiptItems)
            {
                var receiptItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = receiptItem.Customer.Name,
                    CustomerPhoneNumber = receiptItem.Customer.PhoneNumber,
                    Item = receiptItem.Item,
                    ItemBarcode = receiptItem.ItemBarcode,
                    Company = receiptItem.Company,
                    Date = receiptItem.CreatedAt.ToString("yyyy-MM-dd")
                };

                receiptItemsList.Add(receiptItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = receiptItemsList } };
            var result = _pdfExportReportService.GeneratePdf("ReceiptItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> DeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var deliveredItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            paramaters.Add("ContactEmail", "test@gmail.com");
            paramaters.Add("ContactPhoneNumber", "0599854758");
            paramaters.Add("WebsiteLink", "www.test.com");

            var deliveredItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var deliveredItem in deliveredItems)
            {
                var deliveredItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = deliveredItem.Customer.Name,
                    CustomerPhoneNumber = deliveredItem.Customer.PhoneNumber,
                    Item = deliveredItem.Item,
                    ItemBarcode = deliveredItem.ItemBarcode,
                    Company = deliveredItem.Company,
                    Date = deliveredItem.DeliveryDate != null 
                        ? deliveredItem.DeliveryDate.Value.ToString("yyyy-MM-dd") : ""
                };

                deliveredItemsList.Add(deliveredItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = deliveredItemsList } };
            var result = _pdfExportReportService.GeneratePdf("DeliveredItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> ReturnedItemsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.PreviousTechnician)
                .Where(x => x.ReceiptItemType == ReceiptItemType.Returned)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            if (technicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.PreviousTechnicianId != null && x.PreviousTechnicianId.Equals(technicianId));
            }

            var returnedItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            paramaters.Add("ContactEmail", "test@gmail.com");
            paramaters.Add("ContactPhoneNumber", "0599854758");
            paramaters.Add("WebsiteLink", "www.test.com");

            var returnedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var returnedItem in returnedItems)
            {
                var returnedItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = returnedItem.Customer.Name,
                    CustomerPhoneNumber = returnedItem.Customer.PhoneNumber,
                    Item = returnedItem.Item,
                    ItemBarcode = returnedItem.ItemBarcode,
                    Company = returnedItem.Company,
                    Date = returnedItem.CreatedAt.ToString("yyyy-MM-dd"),
                    ReturnReason = returnedItem.ReturnReason ?? "",
                    Technician = returnedItem.PreviousTechnician != null ? returnedItem.PreviousTechnician.FullName : ""
                };

                returnedItemsList.Add(returnedItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = returnedItemsList } };
            var result = _pdfExportReportService.GeneratePdf("ReturnedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> UrgentItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.Urgent)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var urgentItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            paramaters.Add("ContactEmail", "test@gmail.com");
            paramaters.Add("ContactPhoneNumber", "0599854758");
            paramaters.Add("WebsiteLink", "www.test.com");

            var urgentItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var urgentItem in urgentItems)
            {
                var urgentItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = urgentItem.Customer.Name,
                    CustomerPhoneNumber = urgentItem.Customer.PhoneNumber,
                    Item = urgentItem.Item,
                    ItemBarcode = urgentItem.ItemBarcode,
                    Company = urgentItem.Company,
                    Date = urgentItem.CreatedAt.ToString("yyyy-MM-dd")
                };

                urgentItemsList.Add(urgentItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = urgentItemsList } };
            var result = _pdfExportReportService.GeneratePdf("UrgentItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> NotMaintainedItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.New
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.CustomerRefused)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var notMaintainedItems = await dbQuery
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.MaintenanceRequestStatus)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var urgentItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var notMaintainedItem in notMaintainedItems)
            {
                var status = "";
                if (notMaintainedItem.MaintenanceRequestStatus == MaintenanceRequestStatus.New)
                {
                    status = Messages.New;
                }
                else if (notMaintainedItem.MaintenanceRequestStatus == MaintenanceRequestStatus.CustomerRefused)
                {
                    status = Messages.CustomerRefused;
                }

                var notMaintainedItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = notMaintainedItem.Customer.Name,
                    CustomerPhoneNumber = notMaintainedItem.Customer.PhoneNumber,
                    Item = notMaintainedItem.Item,
                    ItemBarcode = notMaintainedItem.ItemBarcode,
                    Company = notMaintainedItem.Company,
                    Date = notMaintainedItem.CreatedAt.ToString("yyyy-MM-dd"),
                    Status = status
                };

                urgentItemsList.Add(notMaintainedItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = notMaintainedItems } };
            var result = _pdfExportReportService.GeneratePdf("NotMaintainedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> NotDeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Completed)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var completedItems = await dbQuery
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var completedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var completedItem in completedItems)
            {
                var completedItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = completedItem.Customer.Name,
                    CustomerPhoneNumber = completedItem.Customer.PhoneNumber,
                    Item = completedItem.Item,
                    ItemBarcode = completedItem.ItemBarcode,
                    Company = completedItem.Company,
                    Date = completedItem.CreatedAt.ToString("yyyy-MM-dd"),
                };

                completedItemsList.Add(completedItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = completedItems } };
            var result = _pdfExportReportService.GeneratePdf("CompletedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> DeliveredItemsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            if (technicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(technicianId));
            }

            var deliveredItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var deliveredItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var deliveredItem in deliveredItems)
            {
                var deliveredItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = deliveredItem.Customer.Name,
                    CustomerPhoneNumber = deliveredItem.Customer.PhoneNumber,
                    Item = deliveredItem.Item,
                    ItemBarcode = deliveredItem.ItemBarcode,
                    Company = deliveredItem.Company,
                    Date = deliveredItem.CreatedAt.ToString("yyyy-MM-dd")
                };

                deliveredItemsList.Add(deliveredItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = deliveredItems } };
            var result = _pdfExportReportService.GeneratePdf("DeliveredItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> CollectedAmountsReport(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.CollectedAmount != null)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            if (technicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(technicianId));
            }

            var collectedAmountItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var collectedAmountItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var collectedAmountItem in collectedAmountItems)
            {
                var collectedAmountItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = collectedAmountItem.Customer.Name,
                    CustomerPhoneNumber = collectedAmountItem.Customer.PhoneNumber,
                    Item = collectedAmountItem.Item,
                    ItemBarcode = collectedAmountItem.ItemBarcode,
                    Company = collectedAmountItem.Company,
                    CollectionDate = collectedAmountItem.CollectionDate != null
                    ? collectedAmountItem.CollectionDate.Value.ToString("yyyy-MM-dd"): "",
                    CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                };

                collectedAmountItemsList.Add(collectedAmountItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = collectedAmountItems } };
            var result = _pdfExportReportService.GeneratePdf("CollectedAmounts.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> SuspendedItemsReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Suspended)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var suspendedItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var suspendedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var suspendedItem in suspendedItems)
            {
                var suspendedItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = suspendedItem.Customer.Name,
                    CustomerPhoneNumber = suspendedItem.Customer.PhoneNumber,
                    Item = suspendedItem.Item,
                    ItemBarcode = suspendedItem.ItemBarcode,
                    Company = suspendedItem.Company,
                    Date = suspendedItem.CreatedAt.ToString("yyyy-MM-dd"),
                    ReturnReason = suspendedItem.MaintenanceSuspensionReason ?? ""
                };

                suspendedItemsList.Add(suspendedItemDataSet);
            }

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = suspendedItems } };
            var result = _pdfExportReportService.GeneratePdf("SuspendedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> TechnicianFeesReport(DateTime? dateFrom, DateTime? dateTo)
        {
            var dbQuery = _db.Users
                .Include(x => x.ReceiptItemsForTechnician)
                .Where(x => x.UserType == UserType.MaintenanceTechnician)
                .AsQueryable();

            if (dateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date >= dateFrom.Value.Date);
            }

            if (dateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt.Date <= dateTo.Value.Date);
            }

            var technicianFees = await dbQuery
                .Select(x => new TechnicianFeesReportDataSet
                {
                    Technician = x.FullName,
                    Fees = x.ReceiptItemsForTechnician.Sum(x => x.CollectedAmount) ?? 0
                }).ToListAsync();

            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd") : "");

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = technicianFees } };
            var result = _pdfExportReportService.GeneratePdf("SuspendedItems.rdlc", dataSets, paramaters);
            return result;
        }

    }
}
