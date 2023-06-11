using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class Branch : BaseEntity
    {
        public Branch()
        {

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<User> Users { get; set; }
        public List<HandReceipt> HandReceipts { get; set; }
        public List<ReturnHandReceipt> ReturnHandReceipts { get; set; }
        public List<HandReceiptItem> HandReceiptItems { get; set; }
        public List<RecipientMaintenance> RecipientMaintenances { get; set; }
        public List<ReturnHandReceiptItem> ReturnHandReceiptItems { get; set; }

    }
}
