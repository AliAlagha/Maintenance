using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnHandReceiptDto
    {
        public int HandReciptId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Date", ResourceType = typeof(Messages))]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Items", ResourceType = typeof(Messages))]
        public List<CreateReturnHandReceiptItemDto> ReturnHandReceiptItems { get; set; }
    }
}
