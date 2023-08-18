using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.ConsoleClient;

public class Worker : IHostedService
{
	private readonly IConsoleClient _dependencyCrawler;
	private readonly ILogger<Worker> _logger;
	private CancellationTokenSource _cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
	private Task? _task;

	public Worker(ILogger<Worker> logger, IConsoleClient dependencyCrawler)
	{
		_logger = logger;
		_dependencyCrawler = dependencyCrawler;
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
			await _task;
		}
		catch (Exception e)
		{
			_logger.LogCritical(e.ToString());
		}
	}
}