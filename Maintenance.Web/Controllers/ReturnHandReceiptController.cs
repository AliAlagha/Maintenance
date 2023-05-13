using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Core.ViewModels;
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

        public IActionResult Index(string? barcode)
        {
            ViewBag.Barcode = barcode;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query, string? barcode)
        {
            var response = await _returnHandReceiptService.GetAll(pagination, query, barcode);
            return Json(response);
        }

        public async Task<List<HandReceiptItemForReturnViewModel>> GetHandReceiptItemsForReturn(int handReceiptId)
        {
            var itemVms = await _returnHandReceiptService.GetHandReceiptItemsForReturn(handReceiptId);
            return itemVms;
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
                if (input.Items.All(x => !x.IsSelected))
                {
                    ModelState.AddModelError("Required", string.Empty);

                    var itemsList = await _returnHandReceiptService.GetHandReceiptItemsForReturn(input.HandReceiptId);
                    ViewBag.HandReceiptItems = itemsList;

                    ViewBag.IsFormValid = false;
                    return View(input);
                }

                await _returnHandReceiptService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }

            var itemVms = await _returnHandReceiptService.GetHandReceiptItemsForReturn(input.HandReceiptId);
            ViewBag.HandReceiptItems = itemVms;

            ViewBag.IsFormValid = false;
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _returnHandReceiptService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

    }
}

