using Maintenance.Core.Enums;

namespace Maintenance.Core.ViewModels
{
    public class UserViewModel : IBaseViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageFilePath { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
    }
}
