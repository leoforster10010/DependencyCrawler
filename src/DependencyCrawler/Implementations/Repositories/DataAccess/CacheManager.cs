using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.DataAccess;

internal class CacheManager : ICacheManager
{
	private readonly ICachedProjectLoader _cachedProjectLoader;
	private readonly ICacher _cacher;
	private readonly IDictionary<Guid, Cache> _caches = new Dictionary<Guid, Cache>();
	private readonly ILogger<CacheManager> _logger;
	private readonly IProjectLoader _projectLoader;
	private Cache? _activeCache;

	public CacheManager(ICacher cacher, ILogger<CacheManager> logger,
		ICachedProjectLoader cachedProjectLoader, IProjectLoader projectLoader)
	{
		_cacher = cacher;
		_logger = logger;
		_cachedProjectLoader = cachedProjectLoader;
		_projectLoader = projectLoader;
	}

	public IReadOnlyList<Cache> Caches => _caches.Values.ToList().AsReadOnly();

	public Cache? ActiveCache
	{
		get => _activeCache;
		private set
		{
			if (value is null)
			{
				return;
			}

			foreach (var cache in _caches.Values.Where(x => x.State == CacheState.Active))
			{
				cache.State = CacheState.Inactive;
			}

			_activeCache = value;
			_activeCache.State = CacheState.Active;

			_projectLoader.LoadProjectsFromCache(_activeCache);
			_logger.LogInformation("Cache activated");
		}
	}

	public void LoadCaches()
	{
		var caches = _cacher.GetAvailableCaches().ToList();
		foreach (var cache in caches)
		{
			if (_caches.TryAdd(cache.Id, cache) && cache.State is CacheState.Active)
			{
				ActiveCache = cache;
			}
		}

		_logger.LogInformation($"{caches.Count} Caches loaded");
	}

	public void ActivateCache(Guid id)
	{
		if (!_caches.TryGetValue(id, out var cache))
		{
			_logger.LogError($"Cache not found: {id}");
			return;
		}

		ActiveCache = cache;
	}


	public void DeleteCache(Guid id)
	{
		if (!_caches.TryGetValue(id, out var cache))
		{
			_logger.LogError($"Cache not found: {id}");
			return;
		}

		if (cache.State is CacheState.Active)
		{
			_logger.LogWarning("Can't delete cache, cache is active!");
			return;
		}

		_caches.Remove(cache.Id);
		_logger.LogInformation($"Cache deleted: {cache.Name}");
	}

	public void SaveAsCurrentCache()
	{
		if (ActiveCache is null)
		{
			_logger.LogWarning("No active cache!");
			return;
		}


		ActiveCache.CachedProjects = _cachedProjectLoader.GetCachedProjects();
		SaveAllCaches();
	}

	public void SaveAsExistingCache(Guid id)
	{
		if (!_caches.TryGetValue(id, out var cache))
		{
			_logger.LogError($"Cache not found: {id}");
			return;
		}


		cache.CachedProjects = _cachedProjectLoader.GetCachedProjects();
		SaveAllCaches();
	}

	public void SaveAsNewCache(string? name = null)
	{
		_cachedProjectLoader.GetCachedProjects();

		var cache = new Cache
		{
			CachedProjects = _cachedProjectLoader.GetCachedProjects(),
			State = CacheState.Inactive,
			Name = name
		};

		_caches.Add(cache.Id, cache);
		SaveAllCaches();
	}

	public void SaveAllCaches()
	{
		_cacher.SaveCaches(_caches.Values);
		_logger.LogInformation($"{_caches.Count} Caches saved");
	}

	public void ReloadCaches()
	{
		_caches.Clear();
		LoadCaches();
	}
}