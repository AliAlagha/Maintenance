using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public interface IHandReceiptService
    {
        Task<PagingResultViewModel<HandReceiptViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<int?> Create(CreateHandReceiptDto input, string userId);
        Task Delete(int id, string userId);

        // Items
        Task<PagingResultViewModel<HandReceiptItemViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, int handReceiptId);
        Task<int> CreateHandReceiptItem(CreateHandReceiptItemDto input, string userId);
        Task DeleteHandReceiptItem(int id, string userId);
    }
}