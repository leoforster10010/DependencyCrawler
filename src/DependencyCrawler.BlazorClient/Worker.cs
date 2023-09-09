using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
using Radzen;

namespace DependencyCrawler.BlazorClient;

public class Worker : IHostedService
{
	private readonly IHostApplicationLifetime _applicationLifetime;
	private readonly ICacheManager _cacheManager;
	private readonly IConfigurationValidator _configurationValidator;
	private readonly ILogger<Worker> _logger;
	private readonly NotificationService? _notificationService;
	private readonly IProjectLoader _projectLoader;
	private CancellationTokenSource _cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
	private Task? _task;

	public Worker(ILogger<Worker> logger, IHostApplicationLifetime applicationLifetime, ICacheManager cacheManager,
		IProjectLoader projectLoader, IConfigurationValidator configurationValidator, IServiceProvider serviceProvider)
	{
		_logger = logger;
		_applicationLifetime = applicationLifetime;
		_cacheManager = cacheManager;
		_projectLoader = projectLoader;
		_configurationValidator = configurationValidator;
		using var scope = serviceProvider.CreateScope();
		_notificationService = scope.ServiceProvider.GetService<NotificationService>();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("StartUp");

		try
		{
			_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			_task = Task.Run(Load, _cts.Token);
			return Task.CompletedTask;
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

	private void Load()
	{
		//ValidateConfiguration();

		_cacheManager.LoadCaches();

		if (!_cacheManager.Caches.Any(x => x.State is CacheState.Active))
		{
			_projectLoader.LoadAllProjects();
		}
	}

	private void ValidateConfiguration()
	{
		while (!_configurationValidator.IsConfigurationValid())
		{
			foreach (var invalidConfiguration in _configurationValidator.GetInvalidConfigurations())
			{
			}
		}
	}
}