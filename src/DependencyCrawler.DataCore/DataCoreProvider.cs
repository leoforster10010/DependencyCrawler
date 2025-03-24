using System.Collections.Concurrent;
using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider : IDataCoreProvider
{
	private readonly ConcurrentDictionary<Guid, IDataCore> _dataCores = new();
	private readonly ILogger<DataCoreProvider> _logger;

	public DataCoreProvider(ILogger<DataCoreProvider> logger)
	{
		_logger = logger;
		ActiveCore = new DataCore(this);
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

		if (!ActiveCore.IsEmpty)
		{
			return dataCore;
		}

		var emptyDataCore = ActiveCore;
		dataCore.Activate();
		emptyDataCore.Delete();
		_logger.LogInformation("Deleted empty DataCore");

		return dataCore;
	}

	public event Action? DataCoreActivated;

	private IDataCore GetOrCreateDataCore(Guid id)
	{
		return _dataCores.ContainsKey(id) ? DataCores[id] : new DataCore(this, id);
	}
}