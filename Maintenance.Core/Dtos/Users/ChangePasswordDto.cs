using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class ChangePasswordDto
    {
        public string Id { get; set; }
        
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Password", ResourceType = typeof(Messages))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
