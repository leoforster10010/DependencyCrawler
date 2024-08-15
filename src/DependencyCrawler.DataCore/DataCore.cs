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
			_dataCoreProvider._dataCores.Add(Id, this);
		}

		public Guid Id { get; } = Guid.NewGuid();
		public bool IsActive => _dataCoreProvider._activeCore?.Id == Id;
		public IReadOnlyDictionary<string, IModule> Modules => _modules.AsReadOnly();
		public IReadOnlyList<IEntity> Entities => _modules.Values.ToList();

		//public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => Modules.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
		//public IReadOnlyDictionary<string, IValueModule> ModulesValue => Modules.ToDictionary(key => key.Key, value => value.Value as IValueModule);
		//public DateTime Timestamp { set; get; } = DateTime.Now;

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
			return new Module(this, name);
		}
	}
}