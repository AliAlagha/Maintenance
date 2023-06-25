﻿using AutoMapper;
using Maintenance.Core.Constants;
using Maintenance.Data;
using Spire.Barcode;

namespace Maintenance.Infrastructure.Services.Barcodes
{
    public class BarcodeService : IBarcodeService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public BarcodeService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public string GenerateBarcode(string barcode, string customerName)
        {
            var bs = new BarcodeSettings
            {
                Type = BarCodeType.Code39,
                Data = barcode,
                ShowText = false,
            };

            var bg = new BarCodeGenerator(bs);

            var barcodeImage = bg.GenerateImage();
            var barcodeFileName = Guid.NewGuid().ToString().Replace("-", "") + ".png";
            var filePath = Path.Combine(Directory.GetCurrentDirectory()
                , "wwwroot", FolderNames.Images, barcodeFileName);
            barcodeImage.Save(filePath);
            return barcodeFileName;
        }

    }
}
