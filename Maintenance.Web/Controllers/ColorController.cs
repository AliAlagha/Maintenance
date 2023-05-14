using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ColorController : BaseController
    {
        private readonly IColorService _colorService;

        public ColorController(IUserService userService
            , IColorService colorService) : base(userService)
        {
            _colorService = colorService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _colorService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateColorDto input)
        {
            if (ModelState.IsValid)
            {
                await _colorService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _colorService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateColorDto input)
        {
            if (ModelState.IsValid)
            {
                await _colorService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _colorService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _colorService.List());
        }
    }
}

