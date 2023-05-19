using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceTechnician")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(IUserService userService
            , ICustomerService customerService) : base(userService)
        {
            _customerService = customerService;
        }

        [Authorize(Roles = "Administrator, MaintenanceTechnician")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, MaintenanceTechnician")]
        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _customerService.GetAll(pagination, query);
            return Json(response);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto input)
        {
            if (ModelState.IsValid)
            {
                await _customerService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(await _customerService.Get(id));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCustomerDto input)
        {
            if (ModelState.IsValid)
            {
                await _customerService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        [Authorize(Roles = "MaintenanceTechnician")]
        public async Task<IActionResult> AddRating(int id)
        {
            return View(await _customerService.GetCustomerRating(id));
        }

        [Authorize(Roles = "MaintenanceTechnician")]
        [HttpPost]
        public async Task<IActionResult> AddRating(AddRatingToCustomerDto input)
        {
            if (ModelState.IsValid)
            {
                await _customerService.AddRating(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _customerService.List());
        }
    }
}

