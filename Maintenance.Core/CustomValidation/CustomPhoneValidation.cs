using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.CustomValidation
{
    internal class CustomPhoneValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var phone = (string)value;
            if (phone != null)
            {
                var isNumeric = int.TryParse(phone, out _);
                var isPhoneValid = isNumeric && phone.Length == 10 && phone.StartsWith("05");
                if (isPhoneValid)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(@Messages.InvalidPhone);
            }

            return ValidationResult.Success;
        }
    }
}
