using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.ReturnHandReceiptItems;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ReturnHandReceiptItemController : BaseController
    {
        private readonly IReturnHandReceiptItemService _returnHandReceiptItemService;

        public ReturnHandReceiptItemController(IUserService userService
            , IReturnHandReceiptItemService returnHandReceiptItemService) : base(userService)
        {
            _returnHandReceiptItemService = returnHandReceiptItemService;
        }

        public async Task<IActionResult> Index(int returnHandReceiptId)
        {
            var isAllItemsDelivered = await _returnHandReceiptItemService.IsAllItemsDelivered(returnHandReceiptId);
            ViewBag.IsAllItemsDelivered = isAllItemsDelivered;
            return View(returnHandReceiptId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query
            , int returnHandReceiptId)
        {
            var response = await _returnHandReceiptItemService.GetAll(pagination, query, returnHandReceiptId);
            return Json(response);
        }

        public async Task<IActionResult> Create(int returnHandReceiptId)
        {
            var handReceiptId = await _returnHandReceiptItemService.GetHandReceiptId(returnHandReceiptId);
            var dto = new CreateReturnItemForExistsReturnHandReceiptDto
            {
                ReturnHandReceiptId = returnHandReceiptId
            };

            ViewBag.HandReceiptId = handReceiptId;
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReturnItemForExistsReturnHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                await _returnHandReceiptItemService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int returnHandReceiptItemId, int returnHandReceiptId)
        {
            var dto = await _returnHandReceiptItemService.Get(returnHandReceiptItemId, returnHandReceiptId);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateReturnHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _returnHandReceiptItemService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int returnHandReceiptItemId, int returnHandReceiptId)
        {
            await _returnHandReceiptItemService.Delete(returnHandReceiptItemId, returnHandReceiptId, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> DeliverItem(int returnHandReceiptItemId
            , int returnHandReceiptId)
        {
            await _returnHandReceiptItemService.DeliverItem(returnHandReceiptItemId, returnHandReceiptId, UserId);
            return RedirectToAction(nameof(Index), new { ReturnHandReceiptId = returnHandReceiptId });
        }

        public async Task<IActionResult> DeliveryOfAllItems(int returnHandReceiptId)
        {
            await _returnHandReceiptItemService.DeliveryOfAllItems(returnHandReceiptId, UserId);
            return RedirectToAction(nameof(Index), new { ReturnHandReceiptId = returnHandReceiptId });
        }

    }
}

