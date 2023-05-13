using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnItemForExistsReturnHandReceiptDto
    {
        public int ReturnHandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "HandReceiptItem", ResourceType = typeof(Messages))]
        public int HandReceiptItemId { get; set; }

        public string? ReturnReason { get; set; }
    }
}
