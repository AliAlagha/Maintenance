using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "FullName", ResourceType = typeof(Messages))]
        public string FullName { get; set; }
        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Email", ResourceType = typeof(Messages))]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(Messages))]
        public string Email { get; set; }
        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Password", ResourceType = typeof(Messages))]
        [DataType(DataType.Password)]
        
        public string Password { get; set; }
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "PhoneNumber", ResourceType = typeof(Messages))]
        [Phone(ErrorMessageResourceName = "InvalidPhone", ErrorMessageResourceType = typeof(Messages))]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "UserType", ResourceType = typeof(Messages))]
        public UserType UserType { get; set; }

        [Display(Name = "Branch", ResourceType = typeof(Messages))]
        public int? BranchId { get; set; }

        [Display(Name = "Image", ResourceType = typeof(Messages))] 
        public IFormFile? ImageFile { get; set; }
        
        [Display(Name = "Activate", ResourceType = typeof(Messages))]
        public bool IsActive { get; set; }
    }
}
