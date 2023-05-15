using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class HandReceiptController : BaseController
    {
        private readonly IHandReceiptService _handReceiptService;

        public HandReceiptController(IUserService userService
            , IHandReceiptService handReceiptService) : base(userService)
        {
            _handReceiptService = handReceiptService;
        }

        public IActionResult Index(string? barcode)
        {
            ViewBag.Barcode = barcode;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAll(Pagination pagination, QueryDto query, string? barcode)
        {
            var response = await _handReceiptService.GetAll(pagination, query, barcode);
            return Json(response);
        }

        public IActionResult SelectCustomerType()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SelectCustomerType(SelectCustomerTypeDto dto)
        {
            if (dto.CreateCustomerType == CreateCustomerType.Exists)
            {
                return RedirectToAction(nameof(Create));
            }
            else if (dto.CreateCustomerType == CreateCustomerType.New)
            {
                return RedirectToAction(nameof(CreateWithCustomer));
            }

            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult CreateWithCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                var isFormValid = true;
                if (input.CustomerId == null || !input.Items.Any())
                {
                    ModelState.AddModelError("Required", string.Empty);
                    isFormValid = false;
                }

                isFormValid = CheckPriceValidity(input, isFormValid);

                if (!isFormValid)
                {
                    ViewBag.IsFormValid = false;
                    return View(input);
                }

                await _handReceiptService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsFormValid = false;
            return View(input);
        }

        private static bool CheckPriceValidity(CreateHandReceiptDto input, bool isFormValid)
        {
            foreach (var item in input.Items)
            {
                var specifiedCost = item.SpecifiedCost;
                var costFrom = item.CostFrom;
                var costTo = item.CostTo;
                var notifyCustomerOfTheCost = item.NotifyCustomerOfTheCost;

                if (specifiedCost.HasValue && (costFrom.HasValue || costTo.HasValue || notifyCustomerOfTheCost))
                {
                    isFormValid = false;
                }

                if (costFrom.HasValue && (specifiedCost.HasValue || costTo.HasValue || notifyCustomerOfTheCost))
                {
                    isFormValid = false;
                }

                if (costTo.HasValue && (specifiedCost.HasValue || costFrom.HasValue || notifyCustomerOfTheCost))
                {
                    isFormValid = false;
                }

                if (notifyCustomerOfTheCost && (specifiedCost.HasValue || costFrom.HasValue || costTo.HasValue))
                {
                    isFormValid = false;
                }
            }

            return isFormValid;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithCustomer(CreateHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                var isCustomerNotValid = input.CustomerInfo == null
                    || input.CustomerInfo.Name == null || input.CustomerInfo.PhoneNumber == null;

                if (isCustomerNotValid || !input.Items.Any())
                {
                    ModelState.AddModelError("Required", string.Empty);
                    ViewBag.IsFormValid = false;
                    return View(input);
                }

                await _handReceiptService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsFormValid = false;
            return View(input);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _handReceiptService.Delete(id, UserId);
            return DeletedSuccessfully();
        }

        public async Task<IActionResult> ExportToPdf(int id)
        {
            var result = await _handReceiptService.ExportToPdf(id);
            return GetPdfFileResult(result, $"{id} - HandReceipt");
        }
    }
}

