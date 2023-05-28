using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class InstantMaintenanceItemViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public double? CollectedAmount { get; set; }
        public UserViewModel Technician { get; set; }
    }
}
