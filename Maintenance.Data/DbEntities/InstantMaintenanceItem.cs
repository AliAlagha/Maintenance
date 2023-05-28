using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class InstantMaintenanceItem : BaseEntity
    {
        public InstantMaintenanceItem()
        {

        }

        public int Id { get; set; }
        public int InstantMaintenanceId { get; set; }
        public InstantMaintenance InstantMaintenance { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public double? CollectedAmount { get; set; }
        public string TechnicianId { get; set; }
        public User Technician { get; set; }
    }
}
