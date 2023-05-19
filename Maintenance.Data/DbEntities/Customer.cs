using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class Customer : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public CustomerRate? CustomerRate { get; set; }
        public string? Notes { get; set; }
        public List<HandReceipt> HandReceipts { get; set; }
        public List<ReturnHandReceipt> ReturnHandReceipts { get; set; }
        public List<ReceiptItem> ReceiptItems { get; set; }
    }
}
