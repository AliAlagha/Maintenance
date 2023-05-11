using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public interface IReturnHandReceiptService
    {
        Task<PagingResultViewModel<ReturnHandReceiptViewModel>> GetAll(Pagination pagination
            , QueryDto query, string? barcode);
        Task<int> Create(CreateReturnHandReceiptDto input, string userId);
        Task Delete(int id, string userId);
        Task<List<HandReceiptItemForReturnViewModel>> GetHandReceiptItemsForReturn(int id);
    }
}