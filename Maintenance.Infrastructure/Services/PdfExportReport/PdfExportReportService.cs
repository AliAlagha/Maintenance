using AutoMapper;
using Maintenance.Core.Constants;
using Maintenance.Core.Dtos;
using Maintenance.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Reflection;

namespace Maintenance.Infrastructure.Services.PdfExportReport
{
    public class PdfExportReportService : IPdfExportReportService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public PdfExportReportService(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public byte[] GeneratePdf(string reportName, List<DataSetDto> dataSets, Dictionary<string, object> reportParameters)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var reportPath = assemblyPath + Path.Combine(FolderNames.Reports, reportName);
            LocalReport report = new LocalReport();
            report.ReportPath = reportPath;

            if (dataSets != null && dataSets.Any())
            {
                foreach (var dataSet in dataSets)
                    report.DataSources.Add(new ReportDataSource(dataSet.Name, dataSet.Data));
            }

            if (reportParameters != null && reportParameters.Any())
            {
                var parameters = reportParameters.Select(x => new ReportParameter(x.Key, x.Value.ToString())).ToList();
                report.SetParameters(parameters);
            }
            return report.Render("PDF");
        }
    }
}
