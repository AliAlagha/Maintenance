using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class SuspenseReceiptItemDto
    {
        public int ReceiptItemId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "MaintenanceSuspensionReason", ResourceType = typeof(Messages))]
        public string? MaintenanceSuspensionReason { get; set; }
    }
}
