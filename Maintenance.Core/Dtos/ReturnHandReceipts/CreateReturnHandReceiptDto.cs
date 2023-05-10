using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnHandReceiptDto
    {
        public int HandReceiptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Items", ResourceType = typeof(Messages))]
        public List<CreateReturnHandReceiptItemDto> Items { get; set; }
    }
}
