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
        DefineMalfunction = 3,
        InformCustomerOfTheCost = 4,
        CustomerApproved = 5,
        CustomerRefused = 6,
        NoResponseFromTheCustomer = 7,
        ItemCannotBeServiced = 8,
        NotifyCustomerOfTheInabilityToMaintain = 9,
        EnterMaintenanceCost = 10,
        Completed = 11,
        NotifyCustomerOfMaintenanceEnd = 12,
        Delivered = 13,
        Suspended = 14,
        RemovedFromMaintained = 15,
    }
}
