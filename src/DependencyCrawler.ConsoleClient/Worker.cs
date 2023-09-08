using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.ConsoleClient;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IConsoleClient _dependencyCrawler;
    private readonly ILogger<Worker> _logger;
    private CancellationTokenSource _cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
    private Task? _task;

    public Worker(ILogger<Worker> logger, IConsoleClient dependencyCrawler,
        IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _dependencyCrawler = dependencyCrawler;
        _applicationLifetime = applicationLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartUp");

        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _task = Task.Run(() => _dependencyCrawler.Run(_cts.Token), _cts.Token);
            return _task;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("ShutDown");

        try
        {
            if (_task is null)
            {
                return;
            }

            _cts.Cancel();

            try
            {
                await _task.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
            }
            catch (Exception)
            {
                _applicationLifetime.StopApplication();
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
        }
    }
}