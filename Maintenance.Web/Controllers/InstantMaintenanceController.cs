using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class InstantMaintenanceController : BaseController
    {
        private readonly IInstantMaintenanceService _instantMaintenanceService;
        private readonly IMapper _mapper;

        public InstantMaintenanceController(IUserService userService
            , IInstantMaintenanceService instantMaintenanceService
            , IMapper mapper) : base(userService)
        {
            _instantMaintenanceService = instantMaintenanceService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _instantMaintenanceService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInstantMaintenanceDto input)
        {
            if (ModelState.IsValid)
            {
                await _instantMaintenanceService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsFormValid = false;
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _instantMaintenanceService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

    }
}

