using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.HandReceiptItems;
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
            var IsAllItemsCanBeDelivered = await _handReceiptItemService.IsAllItemsCanBeDelivered(handReceiptId);
            ViewBag.IsAllItemsCanBeDelivered = IsAllItemsCanBeDelivered;
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
                var isFormValid = CheckPriceValidity(input.SpecifiedCost, input.CostFrom
                    , input.CostTo, input.NotifyCustomerOfTheCost);
                if (!isFormValid)
                {
                    ModelState.AddModelError("PriceNotValid", string.Empty);
                    return View(input);
                }

                await _handReceiptItemService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        //[Authorize(Roles = "Administrator")]
        //public async Task<IActionResult> Edit(int handReceiptItemId, int handReceiptId)
        //{
        //    var dto = await _handReceiptItemService.Get(handReceiptItemId, handReceiptId);
        //    return View(dto);
        //}

        //[Authorize(Roles = "Administrator")]
        //[HttpPost]
        //public async Task<IActionResult> Edit(UpdateHandReceiptItemDto input)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var isFormValid = CheckPriceValidity(input.SpecifiedCost, input.CostFrom
        //            , input.CostTo, input.NotifyCustomerOfTheCost);
        //        if (!isFormValid)
        //        {
        //            ModelState.AddModelError("PriceNotValid", string.Empty);
        //            return View(input);
        //        }

        //        await _handReceiptItemService.Update(input, UserId);
        //        return UpdatedSuccessfully();
        //    }
        //    return View(input);
        //}

        private static bool CheckPriceValidity(double? specifiedCost, double? costFrom
            , double? costTo, bool notifyCustomerOfTheCost)
        {
            var isFormValid = true;
            if (specifiedCost.HasValue && (costFrom.HasValue || costTo.HasValue || notifyCustomerOfTheCost))
            {
                isFormValid = false;
            }

            if (costFrom.HasValue && !costTo.HasValue)
            {
                isFormValid = false;
            }

            if (costTo.HasValue && !costFrom.HasValue)
            {
                isFormValid = false;
            }

            if (costFrom.HasValue && costTo.HasValue && (specifiedCost.HasValue || notifyCustomerOfTheCost))
            {
                isFormValid = false;
            }

            if (notifyCustomerOfTheCost && (specifiedCost.HasValue || costFrom.HasValue || costTo.HasValue))
            {
                isFormValid = false;
            }

            if (!notifyCustomerOfTheCost && !specifiedCost.HasValue && !costFrom.HasValue && !costTo.HasValue)
            {
                isFormValid = false;
            }

            return isFormValid;
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

        public IActionResult RemoveFromMaintained(int handReceiptItemId, int handReceiptId)
        {
            var dto = new RemoveHandItemFromMaintainedDto
            {
                HandReceiptItemId = handReceiptItemId,
                HandReceiptId = handReceiptId
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromMaintained(RemoveHandItemFromMaintainedDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptItemService.RemoveFromMaintained(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

    }
}

