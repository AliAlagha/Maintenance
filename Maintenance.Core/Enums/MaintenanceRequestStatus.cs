using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Enums
{
    public enum MaintenanceRequestStatus
    {
        WaitingManagerResponse = 1,
        ManagerApprovedReturn = 2,
        ManagerRefusedReturn = 3,
        New = 4,
        CheckItem = 5,
        InformCustomerOfTheCost = 6,
        CustomerApproved = 7,
        EnterMaintenanceCost = 8,
        Completed = 9,
        NotifyCustomerOfMaintenanceEnd = 10,
        Delivered = 11,
        CustomerRefused = 12,
        Suspended = 13,
        RemovedFromMaintained = 14
    }
}
