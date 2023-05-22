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
using Maintenance.Core.Helpers;

namespace Maintenance.Infrastructure.Services.ReportsExcel
{
    public class ReportExcelService : IReportExcelService
    {
        private readonly IReportService _reportService;

        public ReportExcelService(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<byte[]> ReceiptItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var receiptItemsList = await _reportService.ReceiptItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)}
            }, new List<ExcelRow>(receiptItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date}
                }
            })));
        }

        public async Task<byte[]> DeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var deliveredItemsList = await _reportService.DeliveredItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)}
            }, new List<ExcelRow>(deliveredItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date}
                }
            })));
        }

        public async Task<byte[]> ReturnedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId };
            var returnedItemsList = await _reportService.ReturnedItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)},
                {"Return Reason", new ExcelColumn("Return Reason", 6)},
                {"Technician", new ExcelColumn("Technician", 7)}
            }, new List<ExcelRow>(returnedItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date},
                    {"Return Reason", e.ReturnReason},
                    {"Technician", e.Technician}
                }
            })));
        }

        public async Task<byte[]> UrgentItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var urgentItemsList = await _reportService.UrgentItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)},
                {"Status", new ExcelColumn("Status", 6)}
            }, new List<ExcelRow>(urgentItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date},
                    {"Status", e.Status}
                }
            })));
        }

        public async Task<byte[]> NotMaintainedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var motMaintainedItemsList = await _reportService.NotMaintainedItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)},
                {"Status", new ExcelColumn("Status", 6)}
            }, new List<ExcelRow>(motMaintainedItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date},
                    {"Status", e.Status}
                }
            })));
        }

        public async Task<byte[]> NotDeliveredItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var completedItemsList = await _reportService.NotDeliveredItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)}
            }, new List<ExcelRow>(completedItemsList.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date}
                }
            })));
        }

        public async Task<byte[]> DeliveredItemsReportByTechnicianExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId };
            var deliveredItems = await _reportService.DeliveredItemsReportByTechnician(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)}
            }, new List<ExcelRow>(deliveredItems.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date}
                }
            })));
        }

        public async Task<byte[]> CollectedAmountsReportExcel(DateTime? dateFrom, DateTime? dateTo
            , string? technicianId)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo, TechnicianId = technicianId };
            var collectedAmountItems = await _reportService.CollectedAmountsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Collection Date", new ExcelColumn("Collection Date", 5)},
                {"Collected Amount", new ExcelColumn("Collected Amount", 5)}
            }, new List<ExcelRow>(collectedAmountItems.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Collection Date", e.CollectionDate},
                    {"Collected Amount", e.CollectedAmount.ToString()}
                }
            })));
        }

        public async Task<byte[]> SuspendedItemsReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var suspendedItems = await _reportService.SuspendedItemsReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Customer Name", new ExcelColumn("Customer Name", 0)},
                {"Customer Phone Number", new ExcelColumn("Customer Phone Number", 1)},
                {"Item", new ExcelColumn("Item", 2)},
                {"Item Barcode", new ExcelColumn("Item Barcode", 3)},
                {"Company", new ExcelColumn("Company", 4)},
                {"Date", new ExcelColumn("Date", 5)},
                {"Return Reason", new ExcelColumn("Return Reason", 6)}
            }, new List<ExcelRow>(suspendedItems.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Customer Name", e.CustomerName},
                    {"Customer Phone Number", e.CustomerPhoneNumber},
                    {"Item",e.Item},
                    {"Item Barcode", e.ItemBarcode},
                    {"Company", e.Company},
                    {"Date", e.Date},
                    {"Return Reason", e.ReturnReason}
                }
            })));
        }

        public async Task<byte[]> TechnicianFeesReportExcel(DateTime? dateFrom, DateTime? dateTo)
        {
            var query = new QueryDto { DateFrom = dateFrom, DateTo = dateTo };
            var technicianFees = await _reportService.TechnicianFeesReport(query);

            return ExcelHelpers.ToExcel(new Dictionary<string, ExcelColumn>
            {
                {"Technician", new ExcelColumn("Technician", 0)},
                {"Fees", new ExcelColumn("Fees", 1)}
            }, new List<ExcelRow>(technicianFees.Select(e => new ExcelRow
            {
                Values = new Dictionary<string, string>
                {
                    {"Technician", e.Technician},
                    {"Fees", e.Fees.ToString()}
                }
            })));
        }

    }
}
