using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class BranchViewModel : IBaseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public BranchViewModel Branch { get; set; }
        public string CreatedAt { get; set; }
    }
}
