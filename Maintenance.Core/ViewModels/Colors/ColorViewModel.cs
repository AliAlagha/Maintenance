using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class ColorViewModel : IBaseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatedAt { get; set; }
    }
}
