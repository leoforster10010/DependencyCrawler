﻿using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider : IDataCoreProvider
{
	private readonly IDictionary<Guid, IDataCore> _dataCores = new ConcurrentDictionary<Guid, IDataCore>();

	public DataCoreProvider()
	{
		ActiveCore = new DataCore(this);
	}

	public DataCoreProvider(DataCoreDTO dataCoreDTO)
	{
		ActiveCore = GetOrCreateDataCore(dataCoreDTO);
	}

	public IDataCore ActiveCore { get; private set; }
	public IReadOnlyDataCore ActiveCoreReadOnly => ActiveCore;
	public IValueDataCore ActiveCoreValue => ActiveCore;
	public IReadOnlyDictionary<Guid, IDataCore> DataCores => _dataCores.AsReadOnly();

	public IDataCore CreateDataCore()
	{
		return new DataCore(this);
	}

	public IDataCore GetOrCreateDataCore(DataCoreDTO dataCoreDto)
	{
		var dataCore = GetOrCreateDataCore(dataCoreDto.Id);

		foreach (var valueModule in dataCoreDto.ModuleValues)
		{
			var module = dataCore.GetOrCreateModule(valueModule.Name);

			foreach (var dependency in valueModule.DependencyValues)
			{
				var dependencyModule = dataCore.GetOrCreateModule(dependency);
				module.AddDependency(dependencyModule);
			}

			foreach (var reference in valueModule.ReferenceValues)
			{
				var referenceModule = dataCore.GetOrCreateModule(reference);
				module.AddReference(referenceModule);
			}
		}

		return dataCore;
	}

	public IDataCore GetOrCreateDataCore(Guid id)
	{
		return _dataCores.ContainsKey(id) ? DataCores[id] : new DataCore(this, id);
	}

	public event Action? DataCoreActivated;
}