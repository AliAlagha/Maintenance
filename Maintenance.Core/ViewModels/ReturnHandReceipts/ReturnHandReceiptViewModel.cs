using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.ViewModels
{
    public class ReturnHandReceiptViewModel : IBaseViewModel
    {
        public int Id { get; set; }
        public HandReceiptViewModel HandReceipt { get; set; }
        public string Date { get; set; }
    }
}
