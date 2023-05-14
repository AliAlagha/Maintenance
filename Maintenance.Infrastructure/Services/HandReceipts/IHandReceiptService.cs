using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public interface IHandReceiptService
    {
        Task<PagingResultViewModel<HandReceiptViewModel>> GetAll(Pagination pagination
            , QueryDto query, string? barcode);
        Task<int?> Create(CreateHandReceiptDto input, string userId);
        Task Delete(int id, string userId);
        Task<byte[]> ExportToPdf(int id);
    }
}