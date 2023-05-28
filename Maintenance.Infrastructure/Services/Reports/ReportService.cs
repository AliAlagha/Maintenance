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
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var handReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var returnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var receiptItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var receiptItem in handReceiptItems)
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

            foreach (var receiptItem in returnHandReceiptItems)
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

            var receiptItemsListOrdered = receiptItemsList.OrderByDescending(x => x.Date).ToList();
            return receiptItemsListOrdered;
        }

        public async Task<List<ReceiptItemReportDataSet>> DeliveredItemsReport(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Delivered)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Delivered)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var deliveredhandReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var deliveredReturnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var deliveredItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var deliveredItem in deliveredhandReceiptItems)
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

            foreach (var deliveredItem in deliveredReturnHandReceiptItems)
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

            var deliveredItemsListOrdered = deliveredItemsList.OrderByDescending(x => x.Date).ToList();
            return deliveredItemsListOrdered;
        }

        public async Task<List<ReceiptItemReportDataSet>> ReturnedItemsReport(QueryDto query)
        {
            var dbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItem)
                .ThenInclude(x => x.Technician)
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
                dbQuery = dbQuery.Where(x => x.HandReceiptItem.TechnicianId != null && x.HandReceiptItem.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.BranchId == query.BranchId);
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
                    Technician = returnedItem.HandReceiptItem.Technician != null ? returnedItem.HandReceiptItem.Technician.FullName : ""
                };

                returnedItemsList.Add(returnedItemDataSet);
            }

            return returnedItemsList;
        }

        public async Task<List<ReceiptItemReportDataSet>> UrgentItemsReport(QueryDto query)
        {
            var dbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.Urgent && x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.New)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.BranchId == query.BranchId);
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
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.New
                    || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.CustomerRefused)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.WaitingManagerResponse
                    || x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.ManagerApprovedReturn
                    || x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn
                    || x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.New)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var notMaintainedHandReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var notMaintainedReturnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var notMaintainedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var notMaintainedItem in notMaintainedHandReceiptItems)
            {
                var status = "";
                status = GetHandReceiptItemRequestStatus(notMaintainedItem, status);

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

                notMaintainedItemsList.Add(notMaintainedItemDataSet);
            }

            foreach (var notMaintainedItem in notMaintainedReturnHandReceiptItems)
            {
                var status = "";
                status = GetReturnHandReceiptItemRequestStatus(notMaintainedItem, status);

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

                notMaintainedItemsList.Add(notMaintainedItemDataSet);
            }

            var notMaintainedItemsListOrdered = notMaintainedItemsList.OrderBy(x => x.Date).ToList();
            return notMaintainedItemsListOrdered;
        }

        private static string GetHandReceiptItemRequestStatus(HandReceiptItem? notMaintainedItem, string status)
        {
            switch (notMaintainedItem.MaintenanceRequestStatus)
            {
                case HandReceiptItemRequestStatus.New:
                    status = $"{Messages.New}";
                    break;
                case HandReceiptItemRequestStatus.CustomerRefused:
                    status = $"{Messages.CustomerRefused} - {notMaintainedItem.ReasonForRefusingMaintenance}";
                    break;
            };
            return status;
        }

        private static string GetReturnHandReceiptItemRequestStatus(ReturnHandReceiptItem? notMaintainedItem, string status)
        {
            switch (notMaintainedItem.MaintenanceRequestStatus)
            {
                case ReturnHandReceiptItemRequestStatus.WaitingManagerResponse:
                    status = $"{Messages.WaitingManagerResponse}";
                    break;
                case ReturnHandReceiptItemRequestStatus.ManagerApprovedReturn:
                    status = $"{Messages.ManagerApprovedReturn}";
                    break;
                case ReturnHandReceiptItemRequestStatus.ManagerRefusedReturn:
                    status = $"{Messages.ManagerRefusedReturn}";
                    break;
                case ReturnHandReceiptItemRequestStatus.New:
                    status = $"{Messages.New}";
                    break;
            };
            return status;
        }

        public async Task<List<ReceiptItemReportDataSet>> NotDeliveredItemsReport(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Completed
                    || x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd)
                .AsQueryable();

            var returnReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Completed
                    || x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.NotifyCustomerOfMaintenanceEnd)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnReceiptItemsDbQuery = returnReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnReceiptItemsDbQuery = returnReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnReceiptItemsDbQuery = returnReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            var completedHandReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var completedReturnHandReceiptItems = await returnReceiptItemsDbQuery
                .ToListAsync();

            var completedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var completedItem in completedHandReceiptItems)
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

            foreach (var completedItem in completedReturnHandReceiptItems)
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

            var completedItemsListOrdered = completedItemsList.OrderByDescending(x => x.Date).ToList();
            return completedItemsListOrdered;
        }

        public async Task<List<ReceiptItemReportDataSet>> DeliveredItemsReportByTechnician(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Delivered)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Delivered)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(query.TechnicianId));
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.TechnicianId != null && x.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var deliveredHandReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var deliveredReturnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var deliveredItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var deliveredItem in deliveredHandReceiptItems)
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

            foreach (var deliveredItem in deliveredReturnHandReceiptItems)
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

            var deliveredItemsListOrdered = deliveredItemsList.OrderByDescending(x => x.Date).ToList();
            return deliveredItemsListOrdered;
        }

        public async Task<List<ReceiptItemReportDataSet>> CollectedAmountsReport(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained)
                .AsQueryable();

            var instantMaintenanceItemsDbQuery = _db.InstantMaintenanceItems
               .Include(x => x.Technician)
               .AsQueryable();

            var recipientMaintenancesDbQuery = _db.RecipientMaintenances
               .Include(x => x.Technician)
               .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null 
                    && x.TechnicianId.Equals(query.TechnicianId));
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var collectedAmountItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var instantMaintenanceItems = await instantMaintenanceItemsDbQuery
                .ToListAsync();
            var recipientMaintenances = await recipientMaintenancesDbQuery
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

            foreach (var collectedAmountItem in instantMaintenanceItems)
            {
                var collectedAmountItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = "---",
                    CustomerPhoneNumber = "---",
                    Item = collectedAmountItem.Item,
                    ItemBarcode = "---",
                    Company = collectedAmountItem.Company,
                    CollectionDate = collectedAmountItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                };

                collectedAmountItemsList.Add(collectedAmountItemDataSet);
            }

            foreach (var collectedAmountItem in recipientMaintenances)
            {
                var collectedAmountItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = "---",
                    CustomerPhoneNumber = "---",
                    Item = "---",
                    ItemBarcode = "---",
                    Company = "---",
                    CollectionDate = collectedAmountItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                };

                collectedAmountItemsList.Add(collectedAmountItemDataSet);
            }

            var collectedAmountItemsListOrdered = collectedAmountItemsList.OrderByDescending(x => x.CollectionDate).ToList();
            return collectedAmountItemsListOrdered;
        }

        public async Task<double> CollectedAmountsReportTotal(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained)
                .AsQueryable();

            var instantMaintenanceItemsDbQuery = _db.InstantMaintenanceItems
               .Include(x => x.Technician)
               .AsQueryable();

            var recipientMaintenancesDbQuery = _db.RecipientMaintenances
               .Include(x => x.Technician)
               .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                instantMaintenanceItemsDbQuery = instantMaintenanceItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var collectedAmountItems = await handReceiptItemsDbQuery
                .SumAsync(x => x.CollectedAmount) ?? 0;
            var instantMaintenanceItems = await instantMaintenanceItemsDbQuery
                .SumAsync(x => x.CollectedAmount) ?? 0;
            var recipientMaintenances = await recipientMaintenancesDbQuery
                .SumAsync(x => x.CollectedAmount) ?? 0;

            var total = collectedAmountItems + instantMaintenanceItems + recipientMaintenances;
            return total;
        }

        public async Task<List<ReceiptItemReportDataSet>> SuspendedItemsReport(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.Suspended)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Where(x => x.MaintenanceRequestStatus == ReturnHandReceiptItemRequestStatus.Suspended)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var suspendedHandReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var suspendedReturnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var suspendedItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var suspendedItem in suspendedHandReceiptItems)
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

            foreach (var suspendedItem in suspendedReturnHandReceiptItems)
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

            var suspendedItemsListOrdered = suspendedItemsList.OrderByDescending(x => x.Date).ToList();
            return suspendedItemsListOrdered;
        }

        public async Task<List<TechnicianFeesReportDataSet>> TechnicianFeesReport(QueryDto query)
        {
            var technicians = await _db.Users
                .Include(x => x.HandReceiptItems.Where(x =>
                x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained
                && (query.DateFrom == null || x.CollectionDate >= query.DateFrom)
                && (query.DateTo == null || x.CollectionDate <= query.DateTo)))
                .Where(x => x.UserType == UserType.MaintenanceTechnician)
                .ToListAsync();

            var technicianFees = technicians
                .Select(x => new TechnicianFeesReportDataSet
                {
                    Technician = x.FullName,
                    Fees = x.HandReceiptItems.Sum(x => x.CollectedAmount) ?? 0
                }).ToList();

            return technicianFees;
        }
    }
}
