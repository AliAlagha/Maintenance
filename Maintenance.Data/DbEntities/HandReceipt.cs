using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class HandReceipt : BaseEntity
    {
        public HandReceipt()
        {
            HandReceiptItems = new List<HandReceiptItem>();
        }

        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public MaintenanceType MaintenanceType { get; set; }
        public List<HandReceiptItem> HandReceiptItems { get; set; }
        public ReturnHandReceipt ReturnHandReceipt { get; set; }
    }
}
