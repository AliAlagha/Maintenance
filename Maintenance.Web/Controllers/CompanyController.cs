using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class CompanyController : BaseController
    {
        private readonly ICompanyService _companyService;

        public CompanyController(IUserService userService
            , ICompanyService companyService) : base(userService)
        {
            _companyService = companyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _companyService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompanyDto input)
        {
            if (ModelState.IsValid)
            {
                await _companyService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _companyService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCompanyDto input)
        {
            if (ModelState.IsValid)
            {
                await _companyService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _companyService.List());
        }
    }
}

