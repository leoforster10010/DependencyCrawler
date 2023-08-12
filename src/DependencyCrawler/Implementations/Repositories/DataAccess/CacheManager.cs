using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models;
using DependencyCrawler.Implementations.Models.LinkedTypes;
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
			_activeCache.State = CacheState.Inactive;

			_projectLoader.LoadProjectsFromCache(_activeCache);
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

internal class State<T, TReadOnly> : Entity
{
	private StateType _stateType;

	private State()
	{
		ParentState = this;
	}

	public State<T, TReadOnly> ParentState { get; init; }
	public Dictionary<Guid, State<T, TReadOnly>> ChildStates { get; } = new();
	public State<T, TReadOnly> ActiveMainState => GetActiveMainState();
	public bool IsRootState => ReferenceEquals(this, ParentState);
	public bool Locked => ChildStates.Any();

	public required StateType StateType
	{
		get => _stateType;
		init => _stateType = value;
	}

	public required IDataContext<T, TReadOnly> DataContext { private get; init; }
	public IReadOnlyDataContext<T, TReadOnly> ReadOnlyDataContext => DataContext;

	public static State<T, TReadOnly> CreateRootState(IDataContext<T, TReadOnly> dataContext)
	{
		return new State<T, TReadOnly>
		{
			StateType = StateType.Main,
			DataContext = dataContext
		};
	}

	public State<T, TReadOnly> CreateChildState()
	{
		if (StateType is StateType.Main)
		{
			var mainState = new State<T, TReadOnly>
			{
				ParentState = this,
				StateType = StateType.Main,
				DataContext = ReadOnlyDataContext.Clone()
			};
			ChildStates.TryAdd(mainState.Id, mainState);
		}

		var branchState = new State<T, TReadOnly>
		{
			ParentState = this,
			StateType = StateType.Branch,
			DataContext = ReadOnlyDataContext.Clone()
		};
		ChildStates.TryAdd(branchState.Id, branchState);

		return branchState;
	}

	public bool TryGetDataContext(out IDataContext<T, TReadOnly> dataContext)
	{
		if (Locked)
		{
			dataContext = new EmptyDataContext<T, TReadOnly>();
			return false;
		}

		dataContext = DataContext;
		return true;
	}

	public void SetAsMainState()
	{
		if (Locked)
		{
			throw new Exception("Can't change the type of locked state!");
		}

		if (StateType is StateType.Main)
		{
			return;
		}

		ActiveMainState.SetBranch();
		SetMain();
	}

	private void SetMain()
	{
		_stateType = StateType.Main;

		if (IsRootState)
		{
			return;
		}

		ParentState.SetMain();
	}

	private void SetBranch()
	{
		_stateType = StateType.Branch;

		if (IsRootState)
		{
			return;
		}

		ParentState.SetBranch();
	}

	private State<T, TReadOnly> GetActiveMainState()
	{
		if (StateType is not StateType.Main)
		{
			return ParentState.GetActiveMainState();
		}

		if (StateType is StateType.Main && !Locked)
		{
			return this;
		}

		var childState = ChildStates.Values.First(x => x.StateType is StateType.Main);
		return childState.GetActiveMainState();
	}
}

internal class EmptyDataContext<T, TReadOnly> : IDataContext<T, TReadOnly>
{
	public TReadOnly DataReadOnly => default!;

	public IDataContext<T, TReadOnly> Clone()
	{
		throw new NotImplementedException();
	}

	public T Data { get; init; } = default!;
}

internal interface IDataContext<T, TReadOnly> : IReadOnlyDataContext<T, TReadOnly>
{
	public T Data { get; init; }
}

internal interface IReadOnlyDataContext<T, TReadOnly>
{
	public TReadOnly DataReadOnly { get; }
	public IDataContext<T, TReadOnly> Clone();
}

internal enum StateType
{
	Main,
	Branch
}