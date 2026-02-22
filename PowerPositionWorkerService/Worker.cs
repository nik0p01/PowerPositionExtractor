using Microsoft.Extensions.Options;

using PowerPositionWorkerService.Options;
using PowerPositionWorkerService.Services;

namespace PowerPositionWorkerService;

internal class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _provider;
    private readonly ExtractOptions _options;

    public Worker(
        ILogger<Worker> logger,
        IServiceProvider provider,
        IOptions<ExtractOptions> options)
    {
        _logger = logger;
        _provider = provider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunExtractAsync();

        using var timer = new PeriodicTimer(
            TimeSpan.FromMinutes(_options.IntervalMinutes));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RunExtractAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Extract failed");
                throw;
            }
        }
    }

    private async Task RunExtractAsync()
    {
        using var scope = _provider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IReportGenerator>();
        await service.GenerateAsync();
    }
}