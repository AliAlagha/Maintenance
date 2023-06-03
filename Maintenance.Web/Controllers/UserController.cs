using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Colors;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : BaseController
    {
        public UserController(IUserService userService) : base(userService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query)
        {
            var response = await _userService.GetAll(pagination, query);
            return Json(response);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto input)
        {
            if (ModelState.IsValid)
            {
                if (input.UserType != UserType.Administrator
                    && input.BranchId == null)
                {
                    ModelState.AddModelError("RequiredError", "");
                    return View(input);
                }

                await _userService.Create(input, UserId);
                return CreatedSuccessfully();
            }
            return View(input);
        }

        public async Task<IActionResult> Edit(string id)
        {
            return View(await _userService.Get(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserDto input)
        {
            if (ModelState.IsValid)
            {
                await _userService.Update(input, UserId);
                return UpdatedSuccessfully();
            }
            return View(input);
        }

        [HttpGet]
        public IActionResult ChangePassword(string id)
        {
            var dto = new ChangePasswordDto
            {
                Id = id
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (ModelState.IsValid)
            {
                await _userService.ChangePassword(dto);
                return UpdatedSuccessfully();
            }

            return View(dto);
        }

        [HttpGet]
        public IActionResult ChangePasswordForUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordForUser(ChangePasswordForUserDto dto)
        {
            if (ModelState.IsValid)
            {
                var isSuccess = await _userService.ChangePasswordForUser(dto, UserId);
                ViewBag.IsSuccess = isSuccess;
                return View();
            }

            return View(dto);
        }

        public async Task<IActionResult> ActivationToggle(string id)
        {
            await _userService.ActivationToggle(id, UserId);
            return UpdatedSuccessfully();
        }

        public async Task<IActionResult> Delete(string id)
        {
            await _userService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> List(UserType? userType)
        {
            return Ok(await _userService.List(userType));
        }
    }
}

