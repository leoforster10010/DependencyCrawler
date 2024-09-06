using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore;

public partial class DataCoreProvider
{
	private partial class DataCore : IDataCore
	{
		private readonly DataCoreProvider _dataCoreProvider;
		private readonly IDictionary<string, IModule> _modules = new Dictionary<string, IModule>();

		public DataCore(DataCoreProvider dataCoreProvider)
		{
			_dataCoreProvider = dataCoreProvider;
			_dataCoreProvider._dataCores.Add(Id, this);
		}

		public Guid Id { get; } = Guid.NewGuid();
		public IDataCoreProvider DataCoreProvider => _dataCoreProvider;
		public bool IsActive => _dataCoreProvider._activeCore == this;
		public IReadOnlyDictionary<string, IModule> Modules => _modules.AsReadOnly();
		public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => _modules.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
		public IReadOnlyList<string> ModuleValues => _modules.Keys.ToList();
		public IReadOnlyList<IEntity> Entities => _modules.Values.ToList();

		public void Activate()
		{
			_dataCoreProvider._activeCore = this;
		}

		public void Delete()
		{
			if (IsActive)
			{
				return;
			}

			_dataCoreProvider._dataCores.Remove(Id);
		}

		public IModule CreateModule(string name)
		{
			return Modules.ContainsKey(name) ? Modules[name] : new Module(this, name);
		}
	}
}