using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Branches;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;

        public BranchController(IUserService userService
            , IBranchService BranchService) : base(userService)
        {
            _branchService = BranchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _branchService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBranchDto input)
        {
            if (ModelState.IsValid)
            {
                await _branchService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _branchService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBranchDto input)
        {
            if (ModelState.IsValid)
            {
                await _branchService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _branchService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List()
        {
            return Ok(await _branchService.List());
        }
    }
}

