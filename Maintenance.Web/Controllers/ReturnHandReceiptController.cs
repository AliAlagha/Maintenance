using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.ReturnHandReceiptItems;
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
            var itemVms = await _returnHandReceiptService.GetHandReceiptItemsForReturn(handReceiptId);
            ViewBag.HandReceiptItems = itemVms;

            var dto = new CreateReturnHandReceiptDto { HandReceiptId = handReceiptId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReturnHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                await _returnHandReceiptService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _returnHandReceiptService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

    }
}

