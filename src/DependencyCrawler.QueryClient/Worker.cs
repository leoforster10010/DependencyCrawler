using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.QueryClient;

internal class Worker(ILogger<Worker> logger, IDependencyCrawler dependencyCrawler) : IHostedService
{
	private CancellationTokenSource _cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
	private Task? _task;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("StartUp");

		try
		{
			_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			_task = Task.Run(dependencyCrawler.Run, _cts.Token);
			return _task;
		}
		catch (Exception e)
		{
			logger.LogCritical(e.ToString());
		}

		return Task.CompletedTask;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("ShutDown");

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
			logger.LogCritical(e.ToString());
		}
	}
}