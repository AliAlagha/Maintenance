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
        DefineMalfunction = 6,
        InformCustomerOfTheCost = 7,
        CustomerApproved = 8,
        CustomerRefused = 9,
        NoResponseFromTheCustomer = 10,
        ItemCannotBeServiced = 11,
        NotifyCustomerOfTheInabilityToMaintain = 12,
        EnterMaintenanceCost = 13,
        Completed = 14,
        NotifyCustomerOfMaintenanceEnd = 15,
        Delivered = 16,
        Suspended = 17,
        RemovedFromMaintained = 18,
    }
}
