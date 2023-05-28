using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class UpdateHandReceiptItemDto
    {
        public int HandReceiptItemId { get; set; }
        public int HandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Barcode", ResourceType = typeof(Messages))]
        public string ItemBarcode { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
        public double? CostNotifiedToTheCustomer { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public bool Urgent { get; set; }
        public int? WarrantyDaysNumber { get; set; }
        public double? CollectedAmount { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        //[Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        //[Display(Name = "Status", ResourceType = typeof(Messages))]
        //public MaintenanceRequestStatus MaintenanceRequestStatus { get; set; }

        public string? ReasonForRefusingMaintenance { get; set; }
        public string? MaintenanceSuspensionReason { get; set; }
        public string? TechnicianId { get; set; }
    }
}
