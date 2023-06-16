using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class ReceiptItemReportDataSet
    {
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string Item { get; set; }
        public string ItemBarcode { get; set; }
        public string Company { get; set; }
        public string Date { get; set; }
        public string ReturnReason { get; set; }
        public string Status { get; set; }
        public double CollectedAmount { get; set; }
        public string CollectionDate { get; set; }
        public string MaintenanceSuspensionReason { get; set; }
        public string Technician { get; set; }
        public string Type { get; set; }
    }
}
