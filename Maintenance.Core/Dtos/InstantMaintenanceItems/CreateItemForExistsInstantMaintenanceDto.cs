﻿using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Maintenance.Core.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Dtos
{
    public class CreateItemForExistsInstantMaintenanceDto
    {
        public int InstantMaintenanceId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Item", ResourceType = typeof(Messages))]
        public int ItemId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Company", ResourceType = typeof(Messages))]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "CollectedAmount", ResourceType = typeof(Messages))]
        public double? CollectedAmount { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Messages))]
        [Display(Name = "Technician", ResourceType = typeof(Messages))]
        public string TechnicianId { get; set; }
    }
}