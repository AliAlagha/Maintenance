using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class CreateReturnHandReceiptItemDto
    {
        public int HandReceiptItemId { get; set; }

        public string? ReturnReason { get; set; }
        public bool IsSelected { get; set; }
        public string TechnicianId { get; set; }
    }
}
