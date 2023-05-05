using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Name", ResourceType = typeof(Messages))]
        public string Name { get; set; }

        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Messages))]
        public string? Email { get; set; }

        [Phone(ErrorMessageResourceName = "InvalidPhone", ErrorMessageResourceType = typeof(Messages))]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
