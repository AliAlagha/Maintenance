using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Enums
{
    public enum HandReceiptItemRequestStatus
    {
        New = 1,
        CheckItem = 2,
        InformCustomerOfTheCost = 3,
        CustomerApproved = 4,
        EnterMaintenanceCost = 5,
        Completed = 6,
        NotifyCustomerOfMaintenanceEnd = 7,
        Delivered = 8,
        CustomerRefused = 9,
        Suspended = 10,
        RemovedFromMaintained = 11
    }
}
