using Axpo;

namespace PowerPositionWorkerService.Services;

internal class PowerTradesServiceClient : ITradesClient
{
    private readonly ILogger<PowerTradesServiceClient> _logger;
    private readonly IPowerService _powerService;

    public PowerTradesServiceClient(ILogger<PowerTradesServiceClient> logger, IPowerService powerService)
    {
        _logger = logger;
        _powerService = powerService;
    }

    public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime tradingDate)
    {
        _logger.LogDebug("Requesting trades for {Date}", tradingDate);
        var trades = await _powerService.GetTradesAsync(tradingDate);

        if (trades == null)
        {
            _logger.LogWarning("No trades returned for {Date}", tradingDate);
            return Enumerable.Empty<PowerTrade>();
        }

        _logger.LogDebug("Got {Count} trades for {Date}", trades.Count(), tradingDate);
        return trades;
    }
}
