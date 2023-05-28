using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.InstantMaintenanceItems;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class InstantMaintenanceItemController : BaseController
    {
        private readonly IInstantMaintenanceItemService _instantMaintenanceItemService;
        private readonly IMapper _mapper;

        public InstantMaintenanceItemController(IUserService userService
            , IInstantMaintenanceItemService instantMaintenanceItemService
            , IMapper mapper) : base(userService)
        {
            _instantMaintenanceItemService = instantMaintenanceItemService;
            _mapper = mapper;
        }

        public IActionResult Index(int instantMaintenanceId)
        {
            return View(instantMaintenanceId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query, int instantMaintenanceId)
        {
            var response = await _instantMaintenanceItemService.GetAll(pagination, query, instantMaintenanceId);
            return Json(response);
        }

        public IActionResult Create(int instantMaintenanceId)
        {
            var dto = new CreateItemForExistsInstantMaintenanceDto { InstantMaintenanceId = instantMaintenanceId};
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateItemForExistsInstantMaintenanceDto input)
        {
            if (ModelState.IsValid)
            {
                await _instantMaintenanceItemService.Create(input, UserId);
                return CreatedSuccessfully();
            }

            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _instantMaintenanceItemService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

    }
}

