using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CollectMoneyForReutrnItemDto
    {
        public int ReturnHandReceiptItemId { get; set; }
        public int ReturnHandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CollectedAmount", ResourceType = typeof(Messages))]
        public double? CollectedAmount { get; set; }
    }
}
