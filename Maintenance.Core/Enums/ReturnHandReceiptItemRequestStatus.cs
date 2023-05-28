using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Enums
{
    public enum ReturnHandReceiptItemRequestStatus
    {
        WaitingManagerResponse = 1,
        ManagerApprovedReturn = 2,
        ManagerRefusedReturn = 3,
        New = 4,
        CheckItem = 5,
        Completed = 6,
        NotifyCustomerOfMaintenanceEnd = 7,
        Delivered = 8,
        Suspended = 9,
        RemovedFromMaintained = 10
    }
}
