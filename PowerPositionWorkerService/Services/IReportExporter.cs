namespace PowerPositionWorkerService.Services;

internal interface IReportExporter
{
    Task ExportAsync(IEnumerable<ReportRow> rows, DateTime dateTimeUtc);
}
