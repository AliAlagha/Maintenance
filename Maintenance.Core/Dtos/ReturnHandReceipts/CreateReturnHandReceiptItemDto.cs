using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnHandReceiptItemDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Item", ResourceType = typeof(Messages))]
        public int ReturnHandReceiptItemId { get; set; }

        public string? ReturnReason { get; set; }
    }
}
