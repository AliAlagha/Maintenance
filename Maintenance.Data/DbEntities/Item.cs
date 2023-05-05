using Maintenance.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Maintenance.Data.DbEntities
{
    public class Item : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
