using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceiptItems;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Maintenance;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class HandReceiptItemController : BaseController
    {
        private readonly IHandReceiptItemService _handReceiptItemService;

        public HandReceiptItemController(IUserService userService
            , IHandReceiptItemService handReceiptItemService) : base(userService)
        {
            _handReceiptItemService = handReceiptItemService;
        }

        public async Task<IActionResult> Index(int handReceiptId)
        {
            var isAllItemsDelivered = await _handReceiptItemService.IsAllItemsDelivered(handReceiptId);
            ViewBag.IsAllItemsDelivered = isAllItemsDelivered;
            return View(handReceiptId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query
            , int handReceiptId)
        {
            var response = await _handReceiptItemService.GetAll(pagination, query, handReceiptId);
            return Json(response);
        }

        public IActionResult Create(int handReceiptId)
        {
            var dto = new CreateHandReceiptItemDto
            {
                HandReceiptId = handReceiptId
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptItemService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int handReceiptItemId, int handReceiptId)
        {
            var dto = await _handReceiptItemService.Get(handReceiptItemId, handReceiptId);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptItemService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int handReceiptItemId, int handReceiptId)
        {
            await _handReceiptItemService.Delete(handReceiptItemId, handReceiptId, UserId);
            return DeletedSuccessfully();
        }

        public IActionResult CollectMoney(int handReceiptItemId, int handReceiptId)
        {
            var dto = new CollectMoneyForHandReceiptItemDto
            {
                HandReceiptItemId = handReceiptItemId,
                HandReceiptId = handReceiptId
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CollectMoney(CollectMoneyForHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptItemService.CollectMoney(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> DeliverItem(int handReceiptItemId, int handReceiptId)
        {
            await _handReceiptItemService.DeliverItem(handReceiptItemId, handReceiptId, UserId);
            return RedirectToAction(nameof(Index), new { HandReceiptId = handReceiptId });
        }

        public async Task<IActionResult> DeliveryOfAllItems(int handReceiptId)
        {
            await _handReceiptItemService.DeliveryOfAllItems(handReceiptId, UserId);
            return RedirectToAction(nameof(Index), new { HandReceiptId = handReceiptId });
        }

    }
}

