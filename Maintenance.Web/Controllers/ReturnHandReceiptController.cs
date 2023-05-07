using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    public class ReturnHandReceiptController : BaseController
    {
        private readonly IReturnHandReceiptService _returnHandReceiptService;

        public ReturnHandReceiptController(IUserService userService
            , IReturnHandReceiptService returnHandReceiptService) : base(userService)
        {
            _returnHandReceiptService = returnHandReceiptService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _returnHandReceiptService.GetAll(pagination, query);
            return Json(response);
        }

        public async Task<IActionResult> Create(int handReceiptId)
        {
            var dto = await _returnHandReceiptService.GetHandReceiptInfo(handReceiptId);
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReturnHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                await _returnHandReceiptService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _returnHandReceiptService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        // Items
        public IActionResult Items(int retunHandReceiptId)
        {
            return View(retunHandReceiptId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllItems(Pagination pagination, QueryDto query
            , int handReceiptId)
        {
            var response = await _returnHandReceiptService.GetAllItems(pagination, query, handReceiptId);
            return Json(response);
        }

        public async Task<IActionResult> DeleteReturnHandReceiptItem(int id)
        {
            await _returnHandReceiptService.DeleteReturnHandReceiptItem(id, UserId);
            return DeletedSuccessfully();
        }

        public IActionResult RetunHandReceiptItemDelivery(int id)
        {
            var dto = new HandReceiptItemDeliveryDto { Id = id };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnHandReceiptItemDelivery(HandReceiptItemDeliveryDto input)
        {
            if (ModelState.IsValid)
            {
                await _returnHandReceiptService.ReturnHandReceiptItemDelivery(input, UserId);
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
                await _returnHandReceiptService.DeliveryOfAllItems(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

    }
}

