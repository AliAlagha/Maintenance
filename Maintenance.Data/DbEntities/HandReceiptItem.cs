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
        public HandReceiptItem()
        {
            MaintenanceRequestStatus = HandReceiptItemRequestStatus.New;
        }

        public int Id { get; set; }
        public int HandReceiptId { get; set; }
        public HandReceipt HandReceipt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
        public double? CostNotifiedToTheCustomer { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public double? FinalCost { get; set; }
        public bool Urgent { get; set; }
        public string ItemBarcode { get; set; }
        public string ItemBarcodeFilePath { get; set; }
        public int? WarrantyDaysNumber { get; set; }
        public double? CollectedAmount { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public HandReceiptItemRequestStatus MaintenanceRequestStatus { get; set; }
        public string? RemoveFromMaintainedReason { get; set; }
        public string? ReasonForRefusingMaintenance { get; set; }
        public string? MaintenanceSuspensionReason { get; set; }
        public string? ReturnReason { get; set; }
        public string? TechnicianId { get; set; }
        public User Technician { get; set; }
        public ReturnHandReceiptItem ReturnHandReceiptItem { get; set; }
    }
}
