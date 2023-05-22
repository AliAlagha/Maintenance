using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class RemoveReturnItemFromMaintainedDto
    {
        public int ReturnHandReceiptItemId { get; set; }
        public int ReturnHandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "RemoveFromMaintainedReason", ResourceType = typeof(Messages))]
        public string? RemoveFromMaintainedReason { get; set; }
    }
}
