using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ReturnHandReceiptItems
{
    public interface IReturnHandReceiptItemService
    {
        Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int returnHandReceiptId);
        Task Delete(int returnHandReceiptItemId, int returnHandReceiptId, string userId);
        Task DeliverItem(int returnHandReceiptItemId, int returnHandReceiptId, string userId);
        Task DeliveryOfAllItems(int returHandReceiptId, string userId);
        Task<bool> IsAllItemsDelivered(int returnHandReceiptId);
    }
}