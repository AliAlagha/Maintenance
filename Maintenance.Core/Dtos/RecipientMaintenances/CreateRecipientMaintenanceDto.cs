using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class CreateRecipientMaintenanceDto
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CollectedAmount", ResourceType = typeof(Messages))]
        public double? CollectedAmount { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CollectedAmountFor", ResourceType = typeof(Messages))]
        public CollectedAmountFor CollectedAmountFor { get; set; }
    }
}
