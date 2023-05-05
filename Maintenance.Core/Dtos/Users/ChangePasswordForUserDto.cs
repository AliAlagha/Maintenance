using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class ChangePasswordForUserDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "OldPassword", ResourceType = typeof(Messages))]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "NewPassword", ResourceType = typeof(Messages))]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
