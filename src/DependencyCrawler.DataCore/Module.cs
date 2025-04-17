using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore;

internal partial class DataCoreProvider
{
	private partial class DataCore
	{
		private class Module : Entity, IModule
		{
			private readonly Dictionary<string, IModule> _dependencies = new();
			private readonly Dictionary<string, IModule> _references = new();


			public Module(DataCore dataCore, string name, ModuleType type) : base(dataCore)
			{
				Name = name;
				Type = type;
				dataCore._modules.Add(Name, this);
			}

			public string Name { get; }
			public ModuleType Type { get; }

			public IReadOnlyDictionary<string, IModule> Dependencies => _dependencies.AsReadOnly();
			public IReadOnlyDictionary<string, IModule> References => _references.AsReadOnly();

			public IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly => new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(_dependencies);
			public IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly => new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(_references);

			public IReadOnlyList<string> DependencyValues => new ReadOnlyListWrapper<string, string>(_dependencies.Keys);
			public IReadOnlyList<string> ReferenceValues => new ReadOnlyListWrapper<string, string>(_references.Keys);

			public int DependencyLayer => !Dependencies.Any() ? 0 : 1 + Dependencies.Max(x => x.Value.DependencyLayer);
			public int ReferenceLayer => !References.Any() ? 0 : 1 + References.Max(x => x.Value.ReferenceLayer);

			public bool IsTopLevel => !Dependencies.Any();
			public bool IsSubLevel => !References.Any();

			public override IReadOnlyList<IEntity> IngoingReferences => new ReadOnlyListWrapper<IModule, IEntity>(_references.Values);
			public override IReadOnlyList<IEntity> OutgoingReferences => new ReadOnlyListWrapper<IModule, IEntity>(_dependencies.Values);

			public void AddDependency(IModule module)
			{
				//same context?
				if (!ReferenceEquals(DataCore, module.DataCore))
				{
					return;
				}

				if (!_dependencies.TryAdd(module.Name, module))
				{
					return;
				}

				module.AddReference(this);
			}

			public void AddReference(IModule module)
			{
				//same context?
				if (!ReferenceEquals(DataCore, module.DataCore))
				{
					return;
				}

				if (!_references.TryAdd(module.Name, module))
				{
					return;
				}

				module.AddDependency(this);
			}

			public void RemoveDependency(IModule module)
			{
				//same context?
				if (!ReferenceEquals(DataCore, module.DataCore))
				{
					return;
				}

				if (!_dependencies.ContainsKey(module.Name))
				{
					return;
				}

				_dependencies.Remove(module.Name);
				module.RemoveReference(this);
			}

			public void RemoveReference(IModule module)
			{
				//same context?
				if (!ReferenceEquals(DataCore, module.DataCore))
				{
					return;
				}

				if (!_references.ContainsKey(module.Name))
				{
					return;
				}

				_references.Remove(module.Name);
				module.RemoveDependency(this);
			}

			public override void Delete()
			{
				foreach (var dependency in _dependencies)
				{
					RemoveDependency(dependency.Value);
				}

				_dataCore._modules.Remove(Name);
			}
		}
	}
}