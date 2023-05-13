using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class ReturnHandReceiptItemViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public string ItemBarcode { get; set; }
        public string? WarrantyExpiryDate { get; set; }
        public string? ReturnReason { get; set; }
        public string? DeliveryDate { get; set; }
        public MaintenanceRequestStatus MaintenanceRequestStatus { get; set; }
        public string MaintenanceRequestStatusMessage { get; set; }
    }
}
