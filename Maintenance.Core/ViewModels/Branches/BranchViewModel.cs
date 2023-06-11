using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class BranchViewModel : IBaseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string CreatedAt { get; set; }
    }
}
