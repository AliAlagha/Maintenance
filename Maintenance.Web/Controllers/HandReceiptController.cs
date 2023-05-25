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
    public class HandReceiptController : BaseController
    {
        private readonly IHandReceiptService _handReceiptService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public HandReceiptController(IUserService userService
            , IHandReceiptService handReceiptService
            , ICustomerService customerService
            , IMapper mapper) : base(userService)
        {
            _handReceiptService = handReceiptService;
            _customerService = customerService;
            _mapper = mapper;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateHandReceiptDto input)
        {
            if (ModelState.IsValid)
            {
                var isFormValid = true;
                isFormValid = ValidateForm(input);

                if (!isFormValid)
                {
                    ModelState.AddModelError("ValidationError", string.Empty);
                    ViewBag.IsFormValid = false;
                    return View(input);
                }

                if (input.CustomerId == null)
                {
                    var createCustomerDto = _mapper.Map<CreateCustomerForHandReceiptDto
                        , CreateCustomerDto>(input.CustomerInfo);
                    input.CustomerId = await _customerService.Create(createCustomerDto, UserId);
                }

                await _handReceiptService.Create(input, UserId);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IsFormValid = false;
            return View(input);
        }

        private bool ValidateForm(CreateHandReceiptDto input)
        {
            bool isFormValid = CheckCustomerValidity(input);
            if (!isFormValid)
            {
                return false;
            }

            isFormValid = CheckPriceValidity(input);
            return isFormValid;
        }

        private bool CheckCustomerValidity(CreateHandReceiptDto input)
        {
            bool isFormValid = true;

            var isCustomerNotValid = input.CustomerId == null
                    && (input.CustomerInfo == null
                        || input.CustomerInfo.Name == null
                        || input.CustomerInfo.PhoneNumber == null);

            if (isCustomerNotValid || !input.Items.Any())
            {
                isFormValid = false;
            }

            return isFormValid;
        }

        private bool CheckPriceValidity(CreateHandReceiptDto input)
        {
            bool isFormValid = true;
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

                if (costFrom.HasValue && !costTo.HasValue)
                {
                    isFormValid = false;
                }

                if (costTo.HasValue && !costFrom.HasValue)
                {
                    isFormValid = false;
                }

                if (costFrom.HasValue && costTo.HasValue && (specifiedCost.HasValue || notifyCustomerOfTheCost))
                {
                    isFormValid = false;
                }

                if (notifyCustomerOfTheCost && (specifiedCost.HasValue || costFrom.HasValue || costTo.HasValue))
                {
                    isFormValid = false;
                }

                if (!notifyCustomerOfTheCost && !specifiedCost.HasValue && !costFrom.HasValue && !costTo.HasValue)
                {
                    isFormValid = false;
                }
            }

            return isFormValid;
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

