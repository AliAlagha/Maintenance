using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class ReturnHandReceiptItem : BaseEntity
    {
        public ReturnHandReceiptItem()
        {
            MaintenanceRequestStatus = ReturnHandReceiptItemRequestStatus.New;
        }

        public int Id { get; set; }
        public int ReturnHandReceiptId { get; set; }
        public ReturnHandReceipt ReturnHandReceipt { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public string ItemBarcode { get; set; }
        public string ItemBarcodeFilePath { get; set; }
        public bool IsReturnItemWarrantyValid { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public ReturnHandReceiptItemRequestStatus MaintenanceRequestStatus { get; set; }
        public string? RemoveFromMaintainedReason { get; set; }
        public string? MaintenanceSuspensionReason { get; set; }
        public string? ReturnReason { get; set; }
        public string? TechnicianId { get; set; }
        public User Technician { get; set; }
        public int HandReceiptItemId { get; set; }
        public HandReceiptItem HandReceiptItem { get; set; }
    }
}
