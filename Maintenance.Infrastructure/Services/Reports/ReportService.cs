using Maintenance.Core.Dtos;
using Maintenance.Data;
using Microsoft.EntityFrameworkCore;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;

        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ReceiptItemReportDataSet>> ReceiptItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var receiptItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    Date = receiptItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
                };

                receiptItemsList.Add(receiptItemDataSet);
            }

            return receiptItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> DeliveredItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var deliveredItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                        ? deliveredItem.DeliveryDate.Value.ToString("yyyy-MM-dd hh:mm tt") : ""
                };

                deliveredItemsList.Add(deliveredItemDataSet);
            }

            return deliveredItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> ReturnedItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.PreviousTechnician)
                .Where(x => x.ReceiptItemType == ReceiptItemType.Returned)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.PreviousTechnicianId != null && x.PreviousTechnicianId.Equals(query.TechnicianId));
            }

            var returnedItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    Date = returnedItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    ReturnReason = returnedItem.ReturnReason ?? "",
                    Technician = returnedItem.PreviousTechnician != null ? returnedItem.PreviousTechnician.FullName : ""
                };

                returnedItemsList.Add(returnedItemDataSet);
            }

            return returnedItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> UrgentItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.Urgent)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var urgentItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    Date = urgentItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
                };

                urgentItemsList.Add(urgentItemDataSet);
            }

            return urgentItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> NotMaintainedItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.WaitingManagerResponse
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.ManagerApprovedReturn
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.ManagerRefusedReturn
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.New
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.CustomerRefused)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var notMaintainedItems = await dbQuery
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.MaintenanceRequestStatus)
                .ToListAsync();

            var motMaintainedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var notMaintainedItem in notMaintainedItems)
            {
                var status = "";
                status = GetRequestStatus(notMaintainedItem, status);

                var notMaintainedItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = notMaintainedItem.Customer.Name,
                    CustomerPhoneNumber = notMaintainedItem.Customer.PhoneNumber,
                    Item = notMaintainedItem.Item,
                    ItemBarcode = notMaintainedItem.ItemBarcode,
                    Company = notMaintainedItem.Company,
                    Date = notMaintainedItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    Status = status
                };

                motMaintainedItemsList.Add(notMaintainedItemDataSet);
            }

            return motMaintainedItemsList;
        }

        private static string GetRequestStatus(ReceiptItem? notMaintainedItem, string status)
        {
            switch (notMaintainedItem.MaintenanceRequestStatus)
            {
                case MaintenanceRequestStatus.WaitingManagerResponse:
                    status = $"{Messages.WaitingManagerResponse}";
                    break;
                case MaintenanceRequestStatus.ManagerApprovedReturn:
                    status = $"{Messages.ManagerApprovedReturn}";
                    break;
                case MaintenanceRequestStatus.ManagerRefusedReturn:
                    status = $"{Messages.ManagerRefusedReturn}";
                    break;
                case MaintenanceRequestStatus.New:
                    status = $"{Messages.New}";
                    break;
                case MaintenanceRequestStatus.CustomerRefused:
                    status = $"{Messages.CustomerRefused} - {notMaintainedItem.ReasonForRefusingMaintenance}";
                    break;
            };
            return status;
        }

        public async Task<List<ReceiptItemReportDataSet>> NotDeliveredItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Completed
                    || x.MaintenanceRequestStatus == MaintenanceRequestStatus.NotifyCustomerOfMaintenanceEnd)
                .OrderByDescending(x => x.CreatedAt)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var completedItems = await dbQuery
                .ToListAsync();

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
                    Date = completedItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                };

                completedItemsList.Add(completedItemDataSet);
            }

            return completedItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> DeliveredItemsReportByTechnician(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Delivered)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(query.TechnicianId));
            }

            var deliveredItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    Technician = deliveredItem.Technician.FullName,
                    Date = deliveredItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt")
                };

                deliveredItemsList.Add(deliveredItemDataSet);
            }

            return deliveredItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> CollectedAmountsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != MaintenanceRequestStatus.RemovedFromMaintained)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                dbQuery = dbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(query.TechnicianId));
            }

            var collectedAmountItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    ? collectedAmountItem.CollectionDate.Value.ToString("yyyy-MM-dd hh:mm tt") : "",
                    CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                };

                collectedAmountItemsList.Add(collectedAmountItemDataSet);
            }

            return collectedAmountItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> SuspendedItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == MaintenanceRequestStatus.Suspended)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            var suspendedItems = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

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
                    Date = suspendedItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    MaintenanceSuspensionReason = suspendedItem.MaintenanceSuspensionReason ?? ""
                };

                suspendedItemsList.Add(suspendedItemDataSet);
            }

            return suspendedItemsList;
        }

        public async Task<List<TechnicianFeesReportDataSet>> TechnicianFeesReport(QueryDto query)
        {
            var technicians = await _db.Users
                .Include(x => x.ReceiptItemsForTechnician.Where(x =>
                x.MaintenanceRequestStatus != MaintenanceRequestStatus.RemovedFromMaintained
                && (query.DateFrom == null || x.CollectionDate >= query.DateFrom)
                && (query.DateTo == null || x.CollectionDate <= query.DateTo)))
                .Where(x => x.UserType == UserType.MaintenanceTechnician)
                .ToListAsync();

            var technicianFees = technicians
                .Select(x => new TechnicianFeesReportDataSet
                {
                    Technician = x.FullName,
                    Fees = x.ReceiptItemsForTechnician.Sum(x => x.CollectedAmount) ?? 0
                }).ToList();

            return technicianFees;
        }
    }
}
