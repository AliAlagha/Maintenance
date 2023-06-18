using Maintenance.Core.Constants;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Infrastructure.Services.Barcodes;
using Maintenance.Infrastructure.Services.Branches;
using Maintenance.Infrastructure.Services.Companies;
using Maintenance.Infrastructure.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Maintenance.Web.Controllers
{
    [Authorize(Roles = "Administrator, MaintenanceManager")]
    public class BarcodeController : BaseController
    {
        private readonly IBarcodeService _barcodeService;

        public BarcodeController(IUserService userService, IBarcodeService barcodeService) : base(userService)
        {
            _barcodeService = barcodeService;
        }

        public IActionResult Scan(BarcodeSearchType type)
        {
            ViewBag.BarcodeSearchType = type;
            return View();
        }

        public IActionResult GenerateBarcode(string barcode, string customerName)
        {
            _barcodeService.GenerateBarcode(barcode, customerName);
            return Ok();
        }

        public async Task<IActionResult> DownloadBarcode(string filePath)
        {
            byte[] fileBytes;
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"
                , FolderNames.Images, $"{filePath}");
            fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(fileBytes, "image/png", filePath);
        }

    }
}

