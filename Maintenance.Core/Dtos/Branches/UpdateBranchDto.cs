using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class UpdateBranchDto
    {
		public int Id { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Name", ResourceType = typeof(Messages))]
        public string Name { get; set; }
    }
}
