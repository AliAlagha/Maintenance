using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class BranchPhoneNumberViewModel : IBaseViewModel
    {
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public string CreatedAt { get; set; }
    }
}
