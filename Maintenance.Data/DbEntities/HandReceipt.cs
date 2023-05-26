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
            ReceiptItems = new List<ReceiptItem>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public List<ReceiptItem> ReceiptItems { get; set; }
        public ReturnHandReceipt ReturnHandReceipt { get; set; }
    }
}
