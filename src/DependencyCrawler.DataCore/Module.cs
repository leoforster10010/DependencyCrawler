using System.Collections.Concurrent;
using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore;

public partial class DataCoreProvider
{
	private partial class DataCore
	{
		private class Module : Entity, IModule
		{
			private readonly IDictionary<string, IModule> _dependencies = new ConcurrentDictionary<string, IModule>();
			private readonly IDictionary<string, IModule> _references = new ConcurrentDictionary<string, IModule>();


			public Module(DataCore dataCore, string name) : base(dataCore)
			{
				Name = name;
				dataCore._modules.Add(Name, this);
			}


			public string Name { get; }
			public IReadOnlyDictionary<string, IModule> Dependencies => _dependencies.AsReadOnly();
			public IReadOnlyDictionary<string, IModule> References => _references.AsReadOnly();
			public IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly => _dependencies.ToDictionary(x => x.Key, y => y.Value as IReadOnlyModule);
			public IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly => _references.ToDictionary(x => x.Key, y => y.Value as IReadOnlyModule);
			public IReadOnlyList<string> DependencyValues => _dependencies.Keys.ToList();
			public IReadOnlyList<string> ReferenceValues => _references.Keys.ToList();
			public int DependencyLayer => !Dependencies.Any() ? 0 : 1 + Dependencies.Max(x => x.Value.DependencyLayer);
			public int ReferenceLayer => !References.Any() ? 0 : 1 + References.Max(x => x.Value.ReferenceLayer);
			public bool IsTopLevel => !Dependencies.Any();
			public bool IsSubLevel => !References.Any();

			public override IReadOnlyList<IEntity> IngoingReferences => _references.Values.ToList();
			public override IReadOnlyList<IEntity> OutgoingReferences => _dependencies.Values.ToList();

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