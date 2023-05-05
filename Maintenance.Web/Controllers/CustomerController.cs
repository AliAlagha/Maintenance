using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(IUserService userService
            , ICustomerService customerService) : base(userService)
        {
            _customerService = customerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _customerService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _customerService.Get(id));
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _customerService.List());
        }
    }
}

