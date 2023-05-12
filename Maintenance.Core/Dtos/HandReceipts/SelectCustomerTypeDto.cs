using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class SelectCustomerTypeDto
    {
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "ExistsOrNewCustomer", ResourceType = typeof(Messages))]
        public CreateCustomerType CreateCustomerType { get; set; }
    }
}
