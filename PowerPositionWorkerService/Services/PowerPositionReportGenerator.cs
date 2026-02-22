namespace PowerPositionWorkerService.Services
{
    internal class PowerPositionReportGenerator : IReportGenerator
    {
        private readonly ILogger<PowerPositionReportGenerator> _logger;
        private readonly IReportExporter _reportExporter;
        private readonly TimeProvider _timeProvider;
        private readonly ITradesClient _tradesClient;
        private readonly TimeZoneInfo _tz;

        public PowerPositionReportGenerator(
            ILogger<PowerPositionReportGenerator> logger,
            IReportExporter reportExporter,
            TimeProvider timeProvider,
            ITradesClient tradesClient,
            TimeZoneInfo tz)
        {
            _logger = logger;
            _reportExporter = reportExporter;
            _timeProvider = timeProvider;
            _tradesClient = tradesClient;
            _tz = tz;
        }

        public async Task GenerateAsync()
        {
            var localDateTimeNow = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow().Date, _tz);

            var tradingDate = GetTradingDate(localDateTimeNow);

            var trades = await _tradesClient.GetTradesAsync(tradingDate);

            var hourlyVolumes = trades
                .SelectMany(t => t.Periods)
                .GroupBy(p => p.Period)
                .Select(g => new
                {
                    Period = g.Key,
                    Volume = g.Sum(x => x.Volume)
                })
                .OrderBy(x => x.Period);

            var rows = hourlyVolumes.Select(h =>
            {
                var hour = GetLocalHour(tradingDate, h.Period, _tz);
                return new ReportRow
                {
                    LocalTime = hour.ToString("HH:mm"),
                    Volume = h.Volume
                };
            });

            await _reportExporter.ExportAsync(rows, localDateTimeNow);

            _logger.LogInformation("Extract completed for {Date}", tradingDate);
        }

        private static DateTime GetLocalHour(DateTime tradingDate, int period, TimeZoneInfo tz)
        {
            var start = tradingDate.AddHours(-1);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(start, tz);
            return TimeZoneInfo.ConvertTimeFromUtc(startUtc.AddHours(period - 1), tz);
        }

        private static DateTime GetTradingDate(DateTime localNow)
        {
            return localNow.Hour >= 23
                ? localNow.Date.AddDays(1)
                : localNow.Date;
        }
    }
}