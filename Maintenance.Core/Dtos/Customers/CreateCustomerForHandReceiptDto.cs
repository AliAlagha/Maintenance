using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateCustomerForHandReceiptDto
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
