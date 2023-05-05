using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class HandReceiptItem : BaseEntity
    {
        public int Id { get; set; }
        public int HandReceiptId { get; set; }
        public HandReceipt HandReceipt { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
        public double? CostNotifiedToTheCustomer { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public bool Urgent { get; set; }
        public string ItemBarcode { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public double? CollectedAmount { get; set; }
        public DateTime? CollectionDate { get; set; }
        public bool Delivered { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public MaintenanceRequestStatus MaintenanceRequestStatus { get; set; }
        public string? ReasonForRefusingMaintenance { get; set; }
    }
}
