using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class ReturnHandReceipt : BaseEntity
    {
        public ReturnHandReceipt()
        {
            ReturnHandReceiptItems = new List<ReturnHandReceiptItem>();
        }

        public int Id { get; set; }
        public int HandReceiptId { get; set; }
        public HandReceipt HandReceipt { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public List<ReturnHandReceiptItem> ReturnHandReceiptItems { get; set; }
    }
}
