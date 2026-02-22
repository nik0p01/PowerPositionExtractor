using Microsoft.Extensions.Options;

using PowerPositionWorkerService.Options;

namespace PowerPositionWorkerService.Services;

internal class PowerPositionReportExporter : IReportExporter
{
    private const string CsvHeader = "Local Time,Volume";
    private readonly ExtractOptions _options;
    private readonly ILogger<PowerPositionReportExporter> _logger;

    public PowerPositionReportExporter(IOptions<ExtractOptions> options, ILogger<PowerPositionReportExporter> logger, TimeZoneInfo tz)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task ExportAsync(IEnumerable<ReportRow> rows, DateTime localNow)
    {
        Directory.CreateDirectory(_options.OutputFolder);

        var filename = $"PowerPosition_{localNow:yyyyMMdd_HHmm}.csv";
        var path = Path.Combine(_options.OutputFolder, filename);

        var lines = new List<string> { CsvHeader };
        lines.AddRange(rows.Select(r => $"{r.LocalTime},{r.Volume}"));

        try
        {
            using var writer = new StreamWriter(path);
            foreach (var line in lines)
            {
                await writer.WriteLineAsync(line);
            }

            _logger.LogInformation("Report written: {Filename} at {Path}", filename, path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write report {Filename} at {Path}", filename, path);
            throw;
        }
    }
}