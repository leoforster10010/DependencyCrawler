using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider : IDataCoreProvider
{
	private readonly Dictionary<Guid, IDataCore> _dataCores = new();
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

		foreach (var valueModule in dataCoreDto.ModuleValues.Values)
		{
			var module = dataCore.GetOrCreateModule(valueModule.Name, valueModule.Type);

			foreach (var dependency in valueModule.DependencyValues)
			{
				if (!dataCoreDto.ModuleValues.TryGetValue(dependency, out var dependencyValueModule))
				{
					continue;
				}

				var dependencyModule = dataCore.GetOrCreateModule(dependency, dependencyValueModule.Type);
				module.AddDependency(dependencyModule);
			}

			foreach (var reference in valueModule.ReferenceValues)
			{
				if (!dataCoreDto.ModuleValues.TryGetValue(reference, out var referenceValueModule))
				{
					continue;
				}

				var referenceModule = dataCore.GetOrCreateModule(reference, referenceValueModule.Type);
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