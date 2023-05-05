using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class HandReceiptViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public CustomerViewModel Customer { get; set; }
        public string Date { get; set; }
        public double? TotalCollectedAmount { get; set; }
    }
}
