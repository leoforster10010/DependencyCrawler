using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal class DependencyCrawlerCoreProvider : IDependencyCrawlerCoreProvider, IDependencyCrawlerDataAccess, IDependencyCrawlerReadonlyDataAccess, IDependencyCrawlerValueDataAccess
{
	private readonly ConcurrentDictionary<Guid, DependencyCrawlerCore> _dependencyCrawlerCores = new();
	private DependencyCrawlerCore? _activeCore;

	public DependencyCrawlerCore ActiveCore
	{
		get
		{
			if (_activeCore is not null)
			{
				return _activeCore;
			}

			_activeCore = new DependencyCrawlerCore
			{
				DependencyCrawlerDataAccess = this
			};

			_dependencyCrawlerCores.TryAdd(_activeCore.Id, _activeCore);
			return _activeCore;
		}
		set
		{
			if (!_dependencyCrawlerCores.ContainsKey(value.Id))
			{
				return;
			}

			_activeCore = value;
		}
	}

	public void AddCore(IDependencyCrawlerValueCore valueCore, bool activate = false)
	{
		var core = new DependencyCrawlerCore
		{
			DependencyCrawlerDataAccess = this
		};

		foreach (var valueModule in valueCore.ModulesValue)
		{
			var module = new Module
			{
				Name = valueModule.Value.NameValue,
				DependencyCrawlerCore = core
			};

			module.DependencyCrawlerCore.Modules.TryAdd(module.Name, module);
		}

		foreach (var valueModule in valueCore.ModulesValue)
		{
			if (!core.Modules.TryGetValue(valueModule.Key, out var module))
			{
				continue;
			}

			foreach (var dependencyOfValue in valueModule.Value.DependencyOfValue)
			{
				module.AddDependencyOf(dependencyOfValue);
			}

			foreach (var dependingOnValue in valueModule.Value.DependingOnValue)
			{
				module.AddDependingOn(dependingOnValue);
			}
		}

		_dependencyCrawlerCores.TryAdd(core.Id, core);

		if (activate)
		{
			ActiveCore = core;
		}
	}

	public void RemoveCore(Guid id)
	{
		if (!_dependencyCrawlerCores.TryGetValue(id, out var core))
		{
			return;
		}

		if (core.IsActive)
		{
			return;
		}

		_dependencyCrawlerCores.TryRemove(core.Id, out _);
	}

	public IDependencyCrawlerCore Core => ActiveCore;
	public IDependencyCrawlerReadonlyCore ReadonlyCore => ActiveCore;
	public IDependencyCrawlerValueCore ValueCore => ActiveCore;
}