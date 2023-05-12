using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateHandReceiptDto
    {
        public int? CustomerId { get; set; }
        public CreateCustomerForHandReceiptDto? CustomerInfo { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Items", ResourceType = typeof(Messages))]
        public List<CreateHandReceiptItemDto> Items { get; set; }
    }
}
