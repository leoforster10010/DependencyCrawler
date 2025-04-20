using DependencyCrawler.Data.Postgresql;
using DependencyCrawler.Data.Postgresql.MigrationService;
using DependencyCrawler.Framework;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<DependencyCrawlerContext>(Constants.PostgresdbName);

builder.Services.AddHostedService<Worker>();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();