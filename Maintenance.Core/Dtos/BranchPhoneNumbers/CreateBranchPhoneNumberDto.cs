using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateBranchPhoneNumberDto
    {
        public int BranchId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Messages))]
        public string PhoneNumber { get; set; }
    }
}
