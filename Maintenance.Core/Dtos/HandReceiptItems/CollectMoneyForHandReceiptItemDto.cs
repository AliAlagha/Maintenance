using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CollectMoneyForHandReceiptItemDto
    {
        public int HandReceiptItemId { get; set; }
        public int HandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CollectedAmount", ResourceType = typeof(Messages))]
        public double? CollectedAmount { get; set; }
    }
}
