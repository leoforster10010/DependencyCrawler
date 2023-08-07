using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories;

public class CacheManager : ICacheManager
{
	private readonly ICachedProjectLoader _cachedProjectLoader;
	private readonly ICachedProjectProvider _cachedProjectProvider;
	private readonly ICacher _cacher;
	private readonly IDictionary<Guid, Cache> _caches = new Dictionary<Guid, Cache>();
	private readonly ILogger<CacheManager> _logger;
	private Cache? _activeCache;

	public CacheManager(ICacher cacher, ICachedProjectProvider cachedProjectProvider, ILogger<CacheManager> logger,
		ICachedProjectLoader cachedProjectLoader)
	{
		_cacher = cacher;
		_cachedProjectProvider = cachedProjectProvider;
		_logger = logger;
		_cachedProjectLoader = cachedProjectLoader;
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
			_activeCache.State = CacheState.Inactive;

			foreach (var cachedProject in _activeCache.CachedProjects)
			{
				_cachedProjectProvider.AddCachedProject(cachedProject);
			}
		}
	}

	public void LoadCaches()
	{
		var caches = _cacher.GetAvailableCaches();
		foreach (var cache in caches)
		{
			if (_caches.TryAdd(cache.Id, cache) && cache.State is CacheState.Active)
			{
				ActiveCache = cache;
			}
		}
	}

	public void ActivateCache(Guid id)
	{
		if (!_caches.TryGetValue(id, out var cache))
		{
			_logger.LogError($"Cache not found: {id}");
			return;
		}

		_cachedProjectProvider.Clear();
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
	}

	public void SaveAsCurrentCache()
	{
		if (ActiveCache is null)
		{
			_logger.LogWarning("No active cache!");
			return;
		}

		_cachedProjectLoader.CacheAllProjects();
		ActiveCache.CachedProjects = _cachedProjectProvider.CachedProjects.ToList();
		SaveAllCaches();
	}

	public void SaveAsExistingCache(Guid id)
	{
		if (!_caches.TryGetValue(id, out var cache))
		{
			_logger.LogError($"Cache not found: {id}");
			return;
		}

		_cachedProjectLoader.CacheAllProjects();
		cache.CachedProjects = _cachedProjectProvider.CachedProjects.ToList();
		SaveAllCaches();
	}

	public void SaveAsNewCache(string? name = null)
	{
		_cachedProjectLoader.CacheAllProjects();

		var cache = new Cache
		{
			CachedProjects = _cachedProjectProvider.CachedProjects.ToList(),
			Name = name
		};

		_caches.Add(cache.Id, cache);
		SaveAllCaches();
	}

	public void SaveAllCaches()
	{
		_cacher.SaveCaches(_caches.Values);
	}

	public void ReloadCaches()
	{
		_caches.Clear();
		LoadCaches();
	}
}