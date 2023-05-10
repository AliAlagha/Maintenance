﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Core.Enums
{
    public enum MaintenanceRequestStatus
    {
        New = 1,
        Suspended = 2,
        CustomerRefused = 3,
        Completed = 4,
        Delivered = 5
    }
}
