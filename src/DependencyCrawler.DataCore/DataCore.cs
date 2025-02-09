using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

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
			_dataCoreProvider._dataCores.Add(Id, this);
		}

		public DataCore(DataCoreProvider dataCoreProvider, Guid id)
		{
			_dataCoreProvider = dataCoreProvider;
			Id = id;
			_dataCoreProvider._dataCores.Add(Id, this);
		}

		public Guid Id { get; }
		public IDataCoreProvider DataCoreProvider => _dataCoreProvider;
		public bool IsActive => _dataCoreProvider.ActiveCore == this;
		public IReadOnlyDictionary<string, IModule> Modules => _modules.AsReadOnly();
		public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => _modules.ToDictionary(key => key.Key, IReadOnlyModule (value) => value.Value);
		public IReadOnlyList<IValueModule> ModuleValues => _modules.Values.ToList();
		public IReadOnlyList<IEntity> Entities => _modules.Values.ToList();

		public void Activate()
		{
			_dataCoreProvider.ActiveCore = this;
			_dataCoreProvider.DataCoreActivated?.Invoke();
		}

		public void Delete()
		{
			if (IsActive)
			{
				return;
			}

			_dataCoreProvider._dataCores.Remove(Id);
		}

		public IModule GetOrCreateModule(string name)
		{
			return Modules.ContainsKey(name) ? Modules[name] : new Module(this, name);
		}

		public DataCoreDTO ToDTO()
		{
			return new DataCoreDTO(this);
		}
	}
}