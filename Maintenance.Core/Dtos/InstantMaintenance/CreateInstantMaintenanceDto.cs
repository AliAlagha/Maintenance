using DocumentFormat.OpenXml.Spreadsheet;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateInstantMaintenanceDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Technician", ResourceType = typeof(Messages))]
        public string TechnicianId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Items", ResourceType = typeof(Messages))]
        public List<CreateInstantMaintenanceItemDto> Items { get; set; }
    }
}
