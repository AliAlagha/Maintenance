using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.ReturnHandReceiptItems
{
    public interface IReturnHandReceiptItemService
    {
        Task<PagingResultViewModel<ReturnHandReceiptItemViewModel>> GetAll(Pagination pagination
            , QueryDto query, int returnHandReceiptId);
        Task<int> Create(CreateReturnItemForExistsReturnHandReceiptDto dto, string userId);
        //Task Update(UpdateReturnHandReceiptItemDto dto, string userId);
        Task Delete(int returnHandReceiptItemId, int returnHandReceiptId, string userId);
        Task CollectMoney(CollectMoneyForReutrnItemDto dto, string userId);
        Task DeliverItem(int returnHandReceiptItemId, int returnHandReceiptId, string userId);
        Task DeliveryOfAllItems(int returHandReceiptId, string userId);
        //Task<bool> IsAllItemsCanBeDelivered(int ReturnHandReceiptId);
        Task<int> GetHandReceiptId(int returnHandReceiptId);
        Task<UpdateReturnHandReceiptItemDto> Get(int returnHandReceiptItemId, int returnHandReceiptId);
        Task RemoveFromMaintained(RemoveReturnItemFromMaintainedDto dto, string userId);
    }
}