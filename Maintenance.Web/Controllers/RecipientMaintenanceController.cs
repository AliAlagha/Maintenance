using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.InstantMaintenanceItems;
using Maintenance.Infrastructure.Services.RecipientMaintenances;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class RecipientMaintenanceController : BaseController
    {
        private readonly IRecipientMaintenanceService _recipientMaintenanceService;
        private readonly IMapper _mapper;

        public RecipientMaintenanceController(IUserService userService
            , IRecipientMaintenanceService recipientMaintenanceService
            , IMapper mapper) : base(userService)
        {
            _recipientMaintenanceService = recipientMaintenanceService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _recipientMaintenanceService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipientMaintenanceDto input)
        {
            if (ModelState.IsValid)
            {
                await _recipientMaintenanceService.Create(input, UserId);
                return CreatedSuccessfully();
            }

            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _recipientMaintenanceService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

    }
}

