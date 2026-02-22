using Axpo;

namespace PowerPositionWorkerService.Services;

internal interface ITradesClient
{
    Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime tradingDate);
}
