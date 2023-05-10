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

    }
}

