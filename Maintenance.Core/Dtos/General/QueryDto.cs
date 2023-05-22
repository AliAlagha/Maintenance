namespace Maintenance.Core.Dtos
{
    public class QueryDto
    {
        public string GeneralSearch { get; set; }
        public int? BranchId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string? TechnicianId { get; set; }
    }
}
