using Maintenance.Core.CustomValidation;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class AddRatingToCustomerDto
    {
		public int Id { get; set; }

        public CustomerRate? CustomerRate { get; set; }
        public string? Notes { get; set; }
    }
}
