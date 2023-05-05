using Maintenance.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Maintenance.Data.DbEntities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public UserType UserType { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDelete { get; set; }
        public bool IsActive { get; set; }

        public User()
        {
            IsDelete = false;
            IsActive = true;
            CreatedAt = DateTime.Now;
        }
    }
}
