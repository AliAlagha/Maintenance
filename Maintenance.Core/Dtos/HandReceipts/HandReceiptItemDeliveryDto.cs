using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class HandReceiptItemDeliveryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "DeliveryDate", ResourceType = typeof(Messages))]
        [DataType(DataType.Date)]
        public DateTime? DeliveryDate { get; set; }
    }
}
