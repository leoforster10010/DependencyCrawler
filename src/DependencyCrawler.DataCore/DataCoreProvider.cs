using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal class DataCoreProvider : IDataCoreProvider, IDataAccess, IReadonlyDataAccess, IValueDataAccess
{
	private readonly ConcurrentDictionary<Guid, DataCore> _dataCores = new();
	private DataCore? _activeCore;

	public IDataCore Core => ActiveCore;

	public DataCore ActiveCore
	{
		get
		{
			if (_activeCore is not null)
			{
				return _activeCore;
			}

			_activeCore = new DataCore
			{
				DataAccess = this
			};

			_dataCores.TryAdd(_activeCore.Id, _activeCore);
			return _activeCore;
		}
		set
		{
			if (!_dataCores.ContainsKey(value.Id))
			{
				return;
			}

			_activeCore = value;
		}
	}

	public void AddCore(IValueDataCore valueCore, bool activate = false)
	{
		if (_dataCores.ContainsKey(valueCore.Id))
		{
			return;
		}

		var core = new DataCore
		{
			DataAccess = this
		};

		foreach (var valueModule in valueCore.ModulesValue)
		{
			var module = new Module
			{
				Name = valueModule.Value.Name,
				DataCore = core,
				Type = valueModule.Value.Type
			};

			module.DataCore.Modules.TryAdd(module.Name, module);
		}

		foreach (var valueModule in valueCore.ModulesValue)
		{
			if (!core.Modules.TryGetValue(valueModule.Key, out var module))
			{
				continue;
			}

			foreach (var referenceValue in valueModule.Value.ReferencesValue)
			{
				module.AddReference(referenceValue);
			}

			foreach (var dependencyValue in valueModule.Value.DependenciesValue)
			{
				module.AddDependency(dependencyValue);
			}
		}

		_dataCores.TryAdd(core.Id, core);

		if (activate)
		{
			ActiveCore = core;
		}
	}

	public void RemoveCore(Guid id)
	{
		if (!_dataCores.TryGetValue(id, out var core) || core.IsActive)
		{
			return;
		}

		_dataCores.TryRemove(core.Id, out _);
	}

	public IReadonlyDataCore ReadonlyCore => ActiveCore;
	public IValueDataCore ValueCore => ActiveCore;
}