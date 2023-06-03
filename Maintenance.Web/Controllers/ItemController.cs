using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Items;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class ItemController : BaseController
    {
        private readonly IItemService _itemService;

        public ItemController(IUserService userService
            , IItemService itemService) : base(userService)
        {
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _itemService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _itemService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _itemService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateItemDto input)
        {
            if (ModelState.IsValid)
            {
                await _itemService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _itemService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _itemService.List());
        }
    }
}

