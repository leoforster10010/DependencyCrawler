using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider
{
	private partial class DataCore : IDataCore
	{
		private readonly DataCoreProvider _dataCoreProvider;
		private readonly IDictionary<string, IModule> _modules = new Dictionary<string, IModule>();

		public DataCore(DataCoreProvider dataCoreProvider)
		{
			_dataCoreProvider = dataCoreProvider;
			Id = Guid.NewGuid();
			_dataCoreProvider._dataCores.TryAdd(Id, this);
		}

		public DataCore(DataCoreProvider dataCoreProvider, Guid id)
		{
			_dataCoreProvider = dataCoreProvider;
			Id = id;
			_dataCoreProvider._dataCores.TryAdd(Id, this);
		}

		public IReadOnlyList<IValueModule> ModuleValues => _modules.Values.ToList();
		public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => _modules.ToDictionary(key => key.Key, IReadOnlyModule (value) => value.Value);

		public Guid Id { get; }
		public IDataCoreProvider DataCoreProvider => _dataCoreProvider;
		public bool IsActive => _dataCoreProvider.ActiveCore == this;
		public IReadOnlyDictionary<string, IModule> Modules => _modules.AsReadOnly();
		public IReadOnlyList<IEntity> Entities => _modules.Values.ToList();

		public void Activate()
		{
			_dataCoreProvider.ActiveCore = this;
			_dataCoreProvider.DataCoreActivated?.Invoke();
			_dataCoreProvider._logger.LogInformation($"DataCore {Id.ToString()} activated.");
		}

		public void Delete()
		{
			if (IsActive)
			{
				_dataCoreProvider._logger.LogWarning("Core is active.");
				return;
			}

			_dataCoreProvider._dataCores.Remove(Id, out _);
			_dataCoreProvider._logger.LogInformation($"DataCore {Id.ToString()} deleted.");
		}

		public IModule GetOrCreateModule(string name)
		{
			return Modules.ContainsKey(name) ? Modules[name] : new Module(this, name);
		}
	}
}