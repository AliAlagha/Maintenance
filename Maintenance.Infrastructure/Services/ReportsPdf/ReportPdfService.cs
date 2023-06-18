using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Exceptions;
using Maintenance.Core.ViewModels;
using Maintenance.Data;
using Maintenance.Data.DbEntities;
using Maintenance.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Maintenance.Infrastructure.Services.Customers;
using Maintenance.Data.Extensions;
using Maintenance.Core.Enums;
using Maintenance.Core.Resources;
using System.Globalization;
using Maintenance.Infrastructure.Services.PdfExportReport;
using Maintenance.Infrastructure.Services.HandReceipts;
using Maintenance.Infrastructure.Services.Reports;
using Microsoft.CodeAnalysis.Operations;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Maintenance.Infrastructure.Services.ReportsPdf
{
    public class ReportPdfService : IReportPdfService
    {
        private readonly IPdfExportReportService _pdfExportReportService;
        private readonly IReportService _reportService;
        private readonly ApplicationDbContext _db;

        public ReportPdfService(IPdfExportReportService pdfExportReportService
            , IReportService reportService, ApplicationDbContext db)
        {
            _pdfExportReportService = pdfExportReportService;
            _reportService = reportService;
            _db = db;
        }

        public async Task<byte[]> ReceiptItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo , BranchId = branchId };
            var receiptItemsList = await _reportService.ReceiptItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = receiptItemsList } };
            var result = _pdfExportReportService.GeneratePdf("ReceiptItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> DeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo , BranchId = branchId };
            var deliveredItemsList = await _reportService.DeliveredItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = deliveredItemsList } };
            var result = _pdfExportReportService.GeneratePdf("DeliveredItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> ReturnedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId , BranchId = branchId };
            var returnedItemsList = await _reportService.ReturnedItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = returnedItemsList } };
            var result = _pdfExportReportService.GeneratePdf("ReturnedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> UrgentItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, BranchId = branchId };
            var urgentItemsList = await _reportService.UrgentItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = urgentItemsList } };
            var result = _pdfExportReportService.GeneratePdf("UrgentItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> NotMaintainedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo , BranchId = branchId };
            var motMaintainedItemsList = await _reportService.NotMaintainedItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = motMaintainedItemsList } };
            var result = _pdfExportReportService.GeneratePdf("NotMaintainedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> NotDeliveredItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, BranchId = branchId };
            var completedItemsList = await _reportService.NotDeliveredItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = completedItemsList } };
            var result = _pdfExportReportService.GeneratePdf("NotDeliveredItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> DeliveredItemsReportByTechnicianPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId , BranchId = branchId };
            var deliveredItems = await _reportService.DeliveredItemsReportByTechnician(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = deliveredItems } };
            var result = _pdfExportReportService.GeneratePdf("DeliveredItemsReportByTechnician.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> CollectedAmountsReportPdf(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId , BranchId = branchId };
            var collectedAmountItems = await _reportService.CollectedAmountsReport(query);
            
            paramaters.Add("TotalCollectedMoney", collectedAmountItems.Sum(x => x.CollectedAmount));

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = collectedAmountItems } };
            var result = _pdfExportReportService.GeneratePdf("CollectedAmounts.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> SuspendedItemsReportPdf(DateTime? dateFrom, DateTime? dateTo, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo , BranchId = branchId };
            var suspendedItems = await _reportService.SuspendedItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = suspendedItems } };
            var result = _pdfExportReportService.GeneratePdf("SuspendedItems.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> TechnicianFeesReportPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var technicianFees = await _reportService.TechnicianFeesReport(query);

            var totalVal = await _reportService.TechnicianFeesReportTotal(query);
            paramaters.Add("TotalTechnicianFees", totalVal);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = technicianFees } };
            var result = _pdfExportReportService.GeneratePdf("TechnicianFees.rdlc", dataSets, paramaters);
            return result;
        }

        public async Task<byte[]> RemovedFromMaintainedItemsReportPdf(string userId, DateTime? dateFrom, DateTime? dateTo
            , string? technicianId, int? branchId)
        {
            var paramaters = new Dictionary<string, object>();
            paramaters.Add("ReportDate", DateTime.Now.ToString("yyyy-MM-dd hh:mm tt"));
            paramaters.Add("DateFrom", dateFrom != null ? dateFrom.Value.ToString("yyyy-MM-dd hh:mm tt") : "");
            paramaters.Add("DateTo", dateTo != null ? dateTo.Value.ToString("yyyy-MM-dd hh:mm tt") : "");

            if (branchId != null)
            {
                var branch = await _db.Branches.SingleOrDefaultAsync(x => x.Id == branchId);
                if (branch == null)
                {
                    throw new EntityNotFoundException();
                }

                paramaters.Add("BranchName", branch.Name);
            }

            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId, BranchId = branchId };
            var itemsList = await _reportService.RemovedFromMaintainedItemsReport(query);

            var dataSets = new List<DataSetDto>() { new DataSetDto { Name = "ReceiptItemReportDataSet", Data = itemsList } };
            var result = _pdfExportReportService.GeneratePdf("RemovedFromMaintainedItems.rdlc", dataSets, paramaters);
            return result;
        }

    }
}
