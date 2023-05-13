using Maintenance.Core.Resources;
using System.ComponentModel.DataAnnotations;

namespace Maintenance.Core.Dtos
{
    public class UpdateReturnHandReceiptItemDto
    {
        public int ReturnHandReceiptId { get; set; }
        public int ReturnHandReceiptItemId { get; set; }
        public string? ReturnReason { get; set; }
    }
}
