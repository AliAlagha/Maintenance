using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class InstantMaintenanceViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public UserViewModel Technician { get; set; }
    }
}
