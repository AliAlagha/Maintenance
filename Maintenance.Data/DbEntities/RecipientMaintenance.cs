using Maintenance.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class RecipientMaintenance : BaseEntity
    {
        public RecipientMaintenance()
        {

        }

        public int Id { get; set; }
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }
        public double? CollectedAmount { get; set; }
        public CollectedAmountFor CollectedAmountFor { get; set; }
    }
}
