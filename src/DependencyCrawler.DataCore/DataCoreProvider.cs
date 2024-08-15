using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal class DataCoreProvider(IDataSourceProvider dataSourceProvider) : IDataCoreProvider, IDataAccess, IReadonlyDataAccess, IValueDataAccess
{
	private readonly ConcurrentDictionary<Guid, DataCore> _dataCores = new();
	private DataCore? _activeCore;

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

	public IDataCore Core => ActiveCore;


	//Todo extract VC -> LC pipeline
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

	//todo should be method on the dataSource
	public void SaveTo(Guid dataSourceId)
	{
		if (!dataSourceProvider.DataSources.TryGetValue(dataSourceId, out var dataSource))
		{
			return;
		}

		dataSource.SaveCores([.._dataCores.Values]);
	}

	public IReadOnlySet<IValueDataCore> ValueDataCores => new HashSet<IValueDataCore>(_dataCores.Values);

	public IReadonlyDataCore ReadonlyCore => ActiveCore;
	public IValueDataCore ValueCore => ActiveCore;

	//ToDo own class DataSourceProvider
	public void Initialize()
	{
		foreach (var valueDataCore in dataSourceProvider.DataSources.Values.Select(dataSource => dataSource.LoadCores()).SelectMany(cores => cores))
		{
			AddCore(valueDataCore);
		}

		var mostRecentCore = _dataCores.Values.MaxBy(x => x.Timestamp);
		if (mostRecentCore is not null)
		{
			ActiveCore = mostRecentCore;
		}
	}
}