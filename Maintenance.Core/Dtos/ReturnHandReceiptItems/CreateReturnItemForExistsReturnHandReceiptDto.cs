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
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public bool Urgent { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
    }
}
