using Axpo;

using PowerPositionWorkerService;
using PowerPositionWorkerService.Options;
using PowerPositionWorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ExtractOptions>(builder.Configuration.GetSection("ExtractOptions"));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(TimeZoneInfo.FindSystemTimeZoneById("Europe/London"));

builder.Services.AddScoped<IReportGenerator, PowerPositionReportGenerator>();
builder.Services.AddScoped<IReportExporter, PowerPositionReportExporter>();
builder.Services.AddScoped<ITradesClient, PowerTradesServiceClient>();

builder.Services.AddScoped<IPowerService, PowerService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
