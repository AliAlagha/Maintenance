using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class ReceiptItemDataSet
    {
        public string Item { get; set; }
        public string ItemBarcode { get; set; }
        public string Company { get; set; }
        public string CollectedAmount { get; set; }
        public string CollectionDate { get; set; }
    }
}
