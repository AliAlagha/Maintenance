using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    public class HandReceiptController : BaseController
    {
        private readonly IHandReceiptService _handReceiptService;

        public HandReceiptController(IUserService userService
            , IHandReceiptService handReceiptService) : base(userService)
        {
            _handReceiptService = handReceiptService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _handReceiptService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _handReceiptService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        // Items
        public IActionResult Items(int handReceiptId)
        {
            return View(handReceiptId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllItems(Pagination pagination, QueryDto query
            , int handReceiptId)
        {
            var response = await _handReceiptService.GetAllItems(pagination, query, handReceiptId);
            return Json(response);
        }

        public IActionResult CreateHandReceiptItem(int handReceiptId)
        {
            var dto = new CreateHandReceiptItemDto
            {
                HandReceiptId = handReceiptId
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHandReceiptItem(CreateHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptService.CreateHandReceiptItem(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> DeleteHandReceiptItem(int id)
        {
            await _handReceiptService.DeleteHandReceiptItem(id, UserId);
            return DeletedSuccessfully();
        }

        public IActionResult CollectMoneyForHandReceiptItem(int id)
        {
            var dto = new CollectMoneyForHandReceiptItemDto { Id = id };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CollectMoneyForHandReceiptItem(CollectMoneyForHandReceiptItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptService.CollectMoneyForHandReceiptItem(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public IActionResult HandReceiptItemDelivery(int id)
        {
            var dto = new HandReceiptItemDeliveryDto { Id = id };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> HandReceiptItemDelivery(HandReceiptItemDeliveryDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptService.HandReceiptItemDelivery(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public IActionResult DeliveryOfAllItems(int id)
        {
            var dto = new DeliveryOfAllItemsDto { Id = id };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryOfAllItems(DeliveryOfAllItemsDto input)
        {
            if (ModelState.IsValid)
            {
                await _handReceiptService.DeliveryOfAllItems(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

    }
}

