using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnHandReceiptItemDto
    {
        public int Index { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Item", ResourceType = typeof(Messages))]
        public int Id { get; set; }

        public string Item { get; set; }
        public string ItemBarcode { get; set; }
        public string Company { get; set; }

        public string? ReturnReason { get; set; }
        public bool IsSelected { get; set; }
    }
}
