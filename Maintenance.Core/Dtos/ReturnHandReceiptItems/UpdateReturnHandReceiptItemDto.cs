using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class UpdateReturnHandReceiptItemDto
    {
        public int ReturnHandReceiptId { get; set; }
        public int ReturnHandReceiptItemId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Barcode", ResourceType = typeof(Messages))]
        public string ItemBarcode { get; set; }

        public string? Description { get; set; }
        public DateTime? DeliveryDate { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Status", ResourceType = typeof(Messages))]
        public MaintenanceRequestStatus MaintenanceRequestStatus { get; set; }

        public string? MaintenanceSuspensionReason { get; set; }
        public string? ReturnReason { get; set; }
        public string? TechnicianId { get; set; }
    }
}
