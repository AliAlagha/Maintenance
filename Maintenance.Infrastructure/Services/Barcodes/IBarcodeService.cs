using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.Services.Barcodes
{
    public interface IBarcodeService
    {
        string GenerateBarcode(string barcode);
    }
}