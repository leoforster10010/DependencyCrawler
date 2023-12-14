using DependencyCrawler.BlazorClient.Contracts;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
using Radzen;

namespace DependencyCrawler.BlazorClient.Backend;

internal class ApplicationStateHandler : IApplicationStateHandler
{
	private readonly ICacheManager _cacheManager;
	private readonly IConfigurationValidator _configurationValidator;
	private readonly NotificationService _notificationService;
	private readonly IProjectLoader _projectLoader;
	private ApplicationState _applicationState;

	public ApplicationStateHandler(IServiceProvider serviceProvider, IProjectLoader projectLoader,
		ICacheManager cacheManager, IConfigurationValidator configurationValidator)
	{
		_projectLoader = projectLoader;
		_cacheManager = cacheManager;
		_configurationValidator = configurationValidator;

		using var scope = serviceProvider.CreateScope();
		_notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

		Load();
	}

	public ApplicationState ApplicationState
	{
		get => _applicationState;
		private set
		{
			NotifyStateChanged();
			_applicationState = value;
		}
	}

	public void Load()
	{
		ApplicationState = ApplicationState.Loading;

		if (!_configurationValidator.IsConfigurationValid())
		{
			ApplicationState = ApplicationState.Unloaded;

			var message =
				"The following configurations are invalid. Please update the appsettings.json accordingly and press reload.";

			foreach (var invalidConfiguration in _configurationValidator.GetInvalidConfigurations())
			{
				message += Environment.NewLine;
				message += invalidConfiguration.ToString();
			}

			_notificationService.Notify(NotificationSeverity.Error, message);

			return;
		}

		_cacheManager.LoadCaches();

		if (!_cacheManager.Caches.Any(x => x.State is CacheState.Active))
		{
			_projectLoader.LoadAllProjects();
		}

		ApplicationState = ApplicationState.Loaded;
	}

	public event Action? OnStateChanged;

	private void NotifyStateChanged()
	{
		OnStateChanged?.Invoke();
	}
}