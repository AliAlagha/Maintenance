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
        public int Id { get; set; }
        public int ReturnHandReceiptId { get; set; }
        public ReturnHandReceipt ReturnHandReceipt { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public string ItemBarcode { get; set; }
        public DateTime? WarrantyExpiryDate { get; set; }
        public string? ReturnReason { get; set; }
        public bool Delivered { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
