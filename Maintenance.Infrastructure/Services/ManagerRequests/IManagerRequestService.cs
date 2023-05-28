using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;

namespace Maintenance.Infrastructure.Services.ManagerRequests
{
    public interface IManagerRequestService
    {
        Task<PagingResultViewModel<ReceiptItemForMaintenanceViewModel>> GetAllItems(Pagination pagination
            , QueryDto query, string userId);
        Task UpdateStatus(int receiptItemId, ReturnHandReceiptItemRequestStatus status
            , string userId);
    }
}