using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class HandReceiptItemForReturnViewModel : IBaseViewModel
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Item { get; set; }
        public string ItemBarcode { get; set; }
        public string Company { get; set; }
        public string? WarrantyDaysNumber { get; set; }
    }
}
