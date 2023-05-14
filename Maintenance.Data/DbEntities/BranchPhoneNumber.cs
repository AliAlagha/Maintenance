using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintenance.Data.DbEntities
{
    public class BranchPhoneNumber : BaseEntity
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
