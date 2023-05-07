using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceipts
{
    public interface IReturnHandReceiptService
    {
        Task<PagingResultViewModel<ReturnHandReceiptViewModel>> GetAll(Pagination pagination, QueryDto query);
        Task<int> Create(CreateReturnHandReceiptDto input, string userId);
        Task Delete(int id, string userId);

        // Items
        Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, int returnHandReceiptId);
        Task DeleteReturnHandReceiptItem(int id, string userId);
        Task ReturnHandReceiptItemDelivery(HandReceiptItemDeliveryDto dto, string userId);
        Task DeliveryOfAllItems(DeliveryOfAllItemsDto dto, string userId);
    }
}