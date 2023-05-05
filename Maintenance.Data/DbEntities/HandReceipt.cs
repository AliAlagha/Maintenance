using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class HandReceipt : BaseEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime Date { get; set; }
        public double? TotalCollectedAmount { get; set; }
        public List<HandReceiptItem> handReceiptItems { get; set; }
    }
}
