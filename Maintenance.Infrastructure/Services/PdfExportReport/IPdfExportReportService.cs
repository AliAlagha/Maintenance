using Maintenance.Core.Dtos;

namespace Maintenance.Infrastructure.Services.PdfExportReport
{
    public interface IPdfExportReportService
    {
        byte[] GeneratePdf(string reportName, List<DataSetDto> dataSets, Dictionary<string, object> reportParameters);
    }
}
