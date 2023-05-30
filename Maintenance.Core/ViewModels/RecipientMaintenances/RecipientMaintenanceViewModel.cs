using DocumentFormat.OpenXml.Spreadsheet;
using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class RecipientMaintenanceViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public double? CollectedAmount { get; set; }
        public CollectedAmountFor CollectedAmountFor { get; set; }
        public string CreatedAt { get; set; }
    }
}
