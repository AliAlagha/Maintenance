using Maintenance.Core.Dtos;
using Maintenance.Infrastructure.Services.Branches;
using Maintenance.Infrastructure.Services.BranchPhoneNumbers;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BranchPhoneNumberController : BaseController
    {
        private readonly IBranchPhoneNumberService _branchPhoneNumberService;

        public BranchPhoneNumberController(IUserService userService
            , IBranchPhoneNumberService branchPhoneNumberService) : base(userService)
        {
            _branchPhoneNumberService = branchPhoneNumberService;
        }

        public IActionResult Index(int branchId)
        {
            return View(branchId);
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query, int branchId)
        {
            var response = await _branchPhoneNumberService.GetAll(pagination, query, branchId);
            return Json(response);
        }

        public IActionResult Create(int branchId)
        {
            var dto = new CreateBranchPhoneNumberDto { BranchId = branchId };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBranchPhoneNumberDto input)
        {
            if (ModelState.IsValid)
            {
                await _branchPhoneNumberService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(int branchPhoneNumberId, int branchId)
        {
            return View(await _branchPhoneNumberService.Get(branchPhoneNumberId, branchId));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBranchPhoneNumberDto input)
        {
            if (ModelState.IsValid)
            {
                await _branchPhoneNumberService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Delete(int branchPhoneNumberId, int branchId)
        {
            await _branchPhoneNumberService.Delete(branchPhoneNumberId, branchId, UserId);
            return DeletedSuccessfully();
        }

    }
}

