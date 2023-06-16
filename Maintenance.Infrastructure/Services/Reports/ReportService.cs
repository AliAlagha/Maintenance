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
                    CustomerName = receiptItem.Customer != null ? receiptItem.Customer.Name : "",
                    CustomerPhoneNumber = receiptItem.Customer != null ? receiptItem.Customer.PhoneNumber: "",
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
                    CustomerName = receiptItem.Customer != null ? receiptItem.Customer.Name : "",
                    CustomerPhoneNumber = receiptItem.Customer != null ? receiptItem.Customer.PhoneNumber : "",
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
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.DeliveryDate >= query.DateFrom.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.DeliveryDate >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.DeliveryDate <= query.DateTo.Value);
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.DeliveryDate <= query.DateTo.Value);
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
                    CustomerName = deliveredItem.Customer != null ? deliveredItem.Customer.Name : "",
                    CustomerPhoneNumber = deliveredItem.Customer != null ? deliveredItem.Customer.PhoneNumber : "",
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
                    CustomerName = deliveredItem.Customer != null ? deliveredItem.Customer.Name : "",
                    CustomerPhoneNumber = deliveredItem.Customer != null ? deliveredItem.Customer.PhoneNumber : "",
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
                    CustomerName = returnedItem.Customer != null ? returnedItem.Customer.Name : "",
                    CustomerPhoneNumber = returnedItem.Customer != null ? returnedItem.Customer.PhoneNumber : "",
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

            var urgentItems = await dbQuery.OrderBy(x => x.CreatedAt)
                .ToListAsync();

            var urgentItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var urgentItem in urgentItems)
            {
                var urgentItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = urgentItem.Customer != null ? urgentItem.Customer.Name : "",
                    CustomerPhoneNumber = urgentItem.Customer != null ? urgentItem.Customer.PhoneNumber : "",
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
                    CustomerName = notMaintainedItem.Customer != null ? notMaintainedItem.Customer.Name : "",
                    CustomerPhoneNumber = notMaintainedItem.Customer != null ? notMaintainedItem.Customer.PhoneNumber : "",
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
                    CustomerName = notMaintainedItem.Customer != null ? notMaintainedItem.Customer.Name : "",
                    CustomerPhoneNumber = notMaintainedItem.Customer != null ? notMaintainedItem.Customer.PhoneNumber : "",
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
                returnReceiptItemsDbQuery = returnReceiptItemsDbQuery.Where(x => x.CreatedAt >= query.DateTo.Value);
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
                returnReceiptItemsDbQuery = returnReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
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
                    CustomerName = completedItem.Customer != null ? completedItem.Customer.Name : "",
                    CustomerPhoneNumber = completedItem.Customer != null ? completedItem.Customer.PhoneNumber : "",
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
                    CustomerName = completedItem.Customer != null ? completedItem.Customer.Name : "",
                    CustomerPhoneNumber = completedItem.Customer != null ? completedItem.Customer.PhoneNumber : "",
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
                    CustomerName = deliveredItem.Customer != null ? deliveredItem.Customer.Name : "",
                    CustomerPhoneNumber = deliveredItem.Customer != null ? deliveredItem.Customer.PhoneNumber : "",
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
                    CustomerName = deliveredItem.Customer != null ? deliveredItem.Customer.Name : "",
                    CustomerPhoneNumber = deliveredItem.Customer != null ? deliveredItem.Customer.PhoneNumber : "",
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

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var collectedAmountItems = await handReceiptItemsDbQuery
                .ToListAsync();

            var collectedAmountItemsList = new List<ReceiptItemReportDataSet>();
            foreach (var collectedAmountItem in collectedAmountItems)
            {
                var collectedAmountItemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = collectedAmountItem.Customer != null ? collectedAmountItem.Customer.Name : "",
                    CustomerPhoneNumber = collectedAmountItem.Customer != null ? collectedAmountItem.Customer.PhoneNumber : "",
                    Item = collectedAmountItem.Item,
                    ItemBarcode = collectedAmountItem.ItemBarcode,
                    Company = collectedAmountItem.Company,
                    CollectionDate = collectedAmountItem.CollectionDate != null
                    ? collectedAmountItem.CollectionDate.Value.ToString("yyyy-MM-dd hh:mm tt") : "",
                    CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                };

                collectedAmountItemsList.Add(collectedAmountItemDataSet);
            }

            await GetRecipientMaintenanceCollectedAmounts(query, collectedAmountItemsList);

            var collectedAmountItemsListOrdered = collectedAmountItemsList.OrderByDescending(x => x.CollectionDate).ToList();
            return collectedAmountItemsListOrdered;
        }

        private async Task GetRecipientMaintenanceCollectedAmounts(QueryDto query, List<ReceiptItemReportDataSet> collectedAmountItemsList)
        {
            if (query.TechnicianId == null)
            {
                var recipientMaintenancesDbQuery = _db.RecipientMaintenances
                .AsQueryable();

                if (query.DateFrom.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                }

                if (query.DateTo.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                }

                if (query.BranchId.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.BranchId == query.BranchId);
                }

                var recipientMaintenances = await recipientMaintenancesDbQuery
                .ToListAsync();
                foreach (var collectedAmountItem in recipientMaintenances)
                {
                    var collectedAmountItemDataSet = new ReceiptItemReportDataSet
                    {
                        CustomerName = "---",
                        CustomerPhoneNumber = "---",
                        Item = Messages.RecipientMaintenance,
                        ItemBarcode = "---",
                        Company = "---",
                        CollectionDate = collectedAmountItem.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                        CollectedAmount = collectedAmountItem.CollectedAmount ?? 0
                    };

                    collectedAmountItemsList.Add(collectedAmountItemDataSet);
                }
            }
        }

        public async Task<double> CollectedAmountsReportTotal(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Where(x => x.CollectedAmount != null
                    && x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained)
                .AsQueryable();

            if (query.DateFrom.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            }

            if (query.DateTo.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            }

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            if (query.BranchId.HasValue)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var collectedAmountItems = await handReceiptItemsDbQuery
                .SumAsync(x => x.CollectedAmount) ?? 0;
            var recipientMaintenances = await GetRecipientMaintenanceCollectedAmountsNumber(query);

            var total = collectedAmountItems + recipientMaintenances;
            return total;
        }

        private async Task<double> GetRecipientMaintenanceCollectedAmountsNumber(QueryDto query)
        {
            double collectedAmounts = 0;
            if (query.TechnicianId == null)
            {
                var recipientMaintenancesDbQuery = _db.RecipientMaintenances
                .AsQueryable();

                if (query.DateFrom.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt >= query.DateFrom.Value);
                }

                if (query.DateTo.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.CreatedAt <= query.DateTo.Value);
                }

                if (query.BranchId.HasValue)
                {
                    recipientMaintenancesDbQuery = recipientMaintenancesDbQuery.Where(x => x.BranchId == query.BranchId);
                }

                collectedAmounts = await recipientMaintenancesDbQuery
                .SumAsync(x => x.CollectedAmount) ?? 0;
            }

            return collectedAmounts;
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
                    CustomerName = suspendedItem.Customer != null ? suspendedItem.Customer.Name : "",
                    CustomerPhoneNumber = suspendedItem.Customer != null ? suspendedItem.Customer.PhoneNumber : "",
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
                    CustomerName = suspendedItem.Customer != null ? suspendedItem.Customer.Name : "",
                    CustomerPhoneNumber = suspendedItem.Customer != null ? suspendedItem.Customer.PhoneNumber : "",
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

        public async Task<List<ReceiptItemReportDataSet>> TechnicianFeesReport(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.CollectedAmount != null)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItem)
                .ThenInclude(x => x.Technician)
                .AsQueryable();

            //if (query.DateFrom.HasValue)
            //{
            //    handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            //    returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            //}

            //if (query.DateTo.HasValue)
            //{
            //    handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            //    returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            //}

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            var handReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var returnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var items = new List<ReceiptItemReportDataSet>();
            foreach (var item in handReceiptItems)
            {
                var itemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = item.Customer != null ? item.Customer.Name : "",
                    CustomerPhoneNumber = item.Customer != null ? item.Customer.PhoneNumber : "",
                    Item = item.Item,
                    ItemBarcode = item.ItemBarcode,
                    Company = item.Company,
                    CollectionDate = item.CollectionDate != null
                    ? item.CollectionDate.Value.ToString("yyyy-MM-dd hh:mm tt") : "",
                    CollectedAmount = item.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained ? item.CollectedAmount.Value : -item.CollectedAmount.Value,
                    Technician = item.Technician != null ? item.Technician.FullName : "",
                    Type = item.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained ? "تم صيانتها" : "مسترجعة"
                };

                items.Add(itemDataSet);
            }

            //foreach (var item in returnHandReceiptItems)
            //{
            //    var itemDataSet = new ReceiptItemReportDataSet
            //    {
            //        CustomerName = item.Customer != null ? item.Customer.Name : "",
            //        CustomerPhoneNumber = item.Customer != null ? item.Customer.PhoneNumber : "",
            //        Item = item.Item,
            //        ItemBarcode = item.ItemBarcode,
            //        Company = item.Company,
            //        CollectionDate = item.CollectionDate != null
            //        ? item.CollectionDate.Value.ToString("yyyy-MM-dd hh:mm tt") : "",
            //        CollectedAmount = item.CollectedAmount ?? 0,
            //        Technician = item.HandReceiptItem.Technician != null ? item.HandReceiptItem.Technician.FullName
            //        : "",
            //        Type = "معادة"
            //    };

            //    items.Add(itemDataSet);
            //}

            var collectedAmountItemsListOrdered = items.OrderByDescending(x => x.CollectionDate).ToList();
            return collectedAmountItemsListOrdered;
        }

        public async Task<double> TechnicianFeesReportTotal(QueryDto query)
        {
            var handReceiptItemsDbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.CollectedAmount != null)
                .AsQueryable();

            var returnHandReceiptItemsDbQuery = _db.ReturnHandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.HandReceiptItem)
                .ThenInclude(x => x.Technician)
                .AsQueryable();

            //if (query.DateFrom.HasValue)
            //{
            //    handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            //    returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CollectionDate >= query.DateFrom.Value);
            //}

            //if (query.DateTo.HasValue)
            //{
            //    handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            //    returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.CollectionDate <= query.DateTo.Value);
            //}

            if (query.TechnicianId != null)
            {
                handReceiptItemsDbQuery = handReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
                returnHandReceiptItemsDbQuery = returnHandReceiptItemsDbQuery.Where(x => x.TechnicianId != null
                    && x.TechnicianId.Equals(query.TechnicianId));
            }

            var handReceiptItems = await handReceiptItemsDbQuery
                .ToListAsync();
            var returnHandReceiptItems = await returnHandReceiptItemsDbQuery
                .ToListAsync();

            var removedFromMaintained = handReceiptItems
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.RemovedFromMaintained).ToList();
            var maintained = handReceiptItems
                .Where(x => x.MaintenanceRequestStatus != HandReceiptItemRequestStatus.RemovedFromMaintained).ToList();

            //var total = maintained.Sum(x => x.CollectedAmount) + returnHandReceiptItems.Sum(x => x.CollectedAmount)
            //    - removedFromMaintained.Sum(x => x.CollectedAmount);
            return 0;
        }

        public async Task<List<ReceiptItemReportDataSet>> RemovedFromMaintainedItemsReport(QueryDto query)
        {
            var dbQuery = _db.HandReceiptItems
                .Include(x => x.Customer)
                .Include(x => x.Technician)
                .Where(x => x.MaintenanceRequestStatus == HandReceiptItemRequestStatus.RemovedFromMaintained)
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

            if (query.BranchId.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.BranchId == query.BranchId);
            }

            var items = await dbQuery.OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var itemsList = new List<ReceiptItemReportDataSet>();
            foreach (var item in items)
            {
                var itemDataSet = new ReceiptItemReportDataSet
                {
                    CustomerName = item.Customer != null ? item.Customer.Name : "",
                    CustomerPhoneNumber = item.Customer != null ? item.Customer.PhoneNumber : "",
                    Item = item.Item,
                    ItemBarcode = item.ItemBarcode,
                    Company = item.Company,
                    Date = item.CreatedAt.ToString("yyyy-MM-dd hh:mm tt"),
                    Technician = item.Technician != null ? item.Technician.FullName : ""
                };

                itemsList.Add(itemDataSet);
            }

            return itemsList;
        }

    }
}
