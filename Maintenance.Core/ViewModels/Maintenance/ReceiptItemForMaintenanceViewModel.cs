using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class ReceiptItemForMaintenanceViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public CustomerViewModel Customer { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
        public double? CostNotifiedToTheCustomer { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public string Urgent { get; set; }
        public string ItemBarcode { get; set; }
        public int? WarrantyDaysNumber { get; set; }
        public string? ReturnReason { get; set; }
        public int MaintenanceRequestStatus { get; set; }
        public string MaintenanceRequestStatusMessage { get; set; }
    }
}
