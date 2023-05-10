using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class EnterMaintenanceCostDto
    {
        public int ReceiptItemId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CostNotifiedToTheCustomer", ResourceType = typeof(Messages))]
        public double? CostNotifiedToTheCustomer { get; set; }
    }
}
