using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class InstantMaintenance : BaseEntity
    {
        public InstantMaintenance()
        {
            InstantMaintenanceItems = new List<InstantMaintenanceItem>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        public string TechnicianId { get; set; }
        public User Technician { get; set; }
        public List<InstantMaintenanceItem> InstantMaintenanceItems { get; set; }
    }
}
