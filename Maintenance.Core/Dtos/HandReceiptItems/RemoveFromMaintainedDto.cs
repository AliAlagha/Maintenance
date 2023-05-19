using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class RemoveFromMaintainedDto
    {
        public int HandReceiptItemId { get; set; }
        public int HandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "RemoveFromMaintainedReason", ResourceType = typeof(Messages))]
        public string? RemoveFromMaintainedReason { get; set; }
    }
}
