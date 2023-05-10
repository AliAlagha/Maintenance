using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.HandReceiptItems
{
    public interface IHandReceiptItemService
    {
        Task<PagingResultViewModel<HandReceiptItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int handReceiptId);
        Task<int> Create(CreateHandReceiptItemDto input, string userId);
        Task Delete(int handReceiptItemId, int handReceiptId, string userId);
        Task CollectMoney(CollectMoneyForHandReceiptItemDto dto, string userId);
        Task DeliverItem(int handReceiptItemId, int handReceiptId, string userId);
        Task DeliveryOfAllItems(int handReceiptId, string userId);
        Task<bool> IsAllItemsDelivered(int handReceiptId);
    }
}