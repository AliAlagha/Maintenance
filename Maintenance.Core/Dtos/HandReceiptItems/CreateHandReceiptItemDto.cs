using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateHandReceiptItemDto
    {
        public int HandReceiptId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Item", ResourceType = typeof(Messages))]
        public int ItemId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Company", ResourceType = typeof(Messages))]
        public int CompanyId { get; set; }

        public int? ColorId { get; set; }
        public string? Description { get; set; }
        public double? SpecifiedCost { get; set; }
        public double? CostFrom { get; set; }
        public double? CostTo { get; set; }
        public bool Urgent { get; set; }
        [DataType(DataType.Date)]
        public DateTime? WarrantyExpiryDate { get; set; }
        public bool NotifyCustomerOfTheCost { get; set; }
    }
}
