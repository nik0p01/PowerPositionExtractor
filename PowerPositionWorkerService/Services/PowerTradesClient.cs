using Axpo;

using Polly;

namespace PowerPositionWorkerService.Services;

internal class PowerTradesClient : ITradesClient
{
    private readonly ILogger<PowerTradesClient> _logger;
    private readonly IPowerService _powerService;

    public PowerTradesClient(ILogger<PowerTradesClient> logger, IPowerService powerService)
    {
        _logger = logger;
        _powerService = powerService;
    }

    public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime tradingDate)
    {
        _logger.LogDebug("Requesting trades for {Date}", tradingDate);

        var retryPolicy = Policy
            .Handle<PowerServiceException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timespan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Error fetching trades (attempt {RetryCount}), will retry after {Delay}s", retryCount, timespan.TotalSeconds);
                });

        var trades = await retryPolicy.ExecuteAsync(() => _powerService.GetTradesAsync(tradingDate));

        if (trades == null)
        {
            _logger.LogWarning("No trades returned for {Date}", tradingDate);
            return [];
        }

        _logger.LogDebug("Got {Count} trades for {Date}", trades.Count(), tradingDate);
        return trades;
    }
}
