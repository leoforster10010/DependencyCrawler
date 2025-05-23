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
			private Dictionary<string, IModule>? _allDependencies;
			private Dictionary<string, IModule>? _allReferences;
			private int? _dependencyLayer;
			private int? _referenceLayer;

			public Module(DataCore dataCore, string name, ModuleType type) : base(dataCore)
			{
				Name = name;
				Type = type;
				dataCore._modules.Add(Name, this);
			}

			public string Name { get; }
			public ModuleType Type { get; }

			public int DependencyLayer
			{
				get
				{
					_dependencyLayer ??= IsTopLevel ? 0 : 1 + DependenciesReadOnly.Max(x => x.Value.DependencyLayer);
					return _dependencyLayer.Value;
				}
			}

			public int ReferenceLayer
			{
				get
				{
					_referenceLayer ??= IsSubLevel ? 0 : 1 + ReferencesReadOnly.Max(x => x.Value.ReferenceLayer);
					return _referenceLayer.Value;
				}
			}

			public bool IsTopLevel => !Dependencies.Any();
			public bool IsSubLevel => !References.Any();

			public IReadOnlyDictionary<string, IModule> Dependencies => _dependencies.AsReadOnly();
			public IReadOnlyDictionary<string, IModule> References => _references.AsReadOnly();

			public IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly => new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(_dependencies);
			public IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly => new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(_references);

			public IReadOnlyList<string> DependencyValues => new ReadOnlyListWrapper<string, string>(_dependencies.Keys);
			public IReadOnlyList<string> ReferenceValues => new ReadOnlyListWrapper<string, string>(_references.Keys);

			public override IReadOnlyList<IEntity> IngoingReferences => new ReadOnlyListWrapper<IModule, IEntity>(_references.Values);
			public override IReadOnlyList<IEntity> OutgoingReferences => new ReadOnlyListWrapper<IModule, IEntity>(_dependencies.Values);

			public IReadOnlyDictionary<string, IModule> GetAllDependencies()
			{
				if (_allDependencies is not null)
				{
					return _allDependencies;
				}

				_allDependencies ??= new Dictionary<string, IModule>();
				foreach (var dependency in Dependencies)
				{
					if (_allDependencies.ContainsKey(dependency.Key))
					{
						continue;
					}

					_allDependencies[dependency.Key] = dependency.Value;

					foreach (var module in dependency.Value.GetAllDependencies())
					{
						_allDependencies.TryAdd(module.Key, module.Value);
					}
				}

				return _allDependencies;
			}

			public IReadOnlyDictionary<string, IModule> GetAllReferences()
			{
				if (_allReferences is not null)
				{
					return _allReferences;
				}

				_allReferences ??= new Dictionary<string, IModule>();
				foreach (var reference in References)
				{
					if (_allReferences.ContainsKey(reference.Key))
					{
						continue;
					}

					_allReferences[reference.Key] = reference.Value;

					foreach (var module in reference.Value.GetAllReferences())
					{
						_allReferences.TryAdd(module.Key, module.Value);
					}
				}

				return _allReferences;
			}

			public bool ReferencedBy(IValueModule module)
			{
				return this == module || GetAllReferences().ContainsKey(module.Name);
			}

			public bool DependsOn(IValueModule module)
			{
				return this == module || GetAllDependencies().ContainsKey(module.Name);
			}

			public IReadOnlyDictionary<string, IReadOnlyModule> GetAllDependenciesReadOnly()
			{
				return new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(GetAllDependencies());
			}

			public IReadOnlyDictionary<string, IReadOnlyModule> GetAllReferencesReadOnly()
			{
				return new ReadOnlyDictionaryWrapper<string, IReadOnlyModule, IModule>(GetAllReferences());
			}

			public IDictionary<string, HashSet<string>> GetRedundantDependencies(IDictionary<string, HashSet<string>>? unusedDependencies = null, HashSet<string>? visited = null)
			{
				unusedDependencies ??= new Dictionary<string, HashSet<string>>();
				visited ??= [];

				if (!visited.Add(Name))
				{
					return unusedDependencies;
				}

				if (!unusedDependencies.ContainsKey(Name))
				{
					unusedDependencies[Name] = [];
				}

				foreach (var module in DependenciesReadOnly)
				{
					if (DependenciesReadOnly.Where(otherModule => module.Key != otherModule.Key).Any(otherModule => otherModule.Value.DependsOn(module.Value)))
					{
						unusedDependencies[Name].Add(module.Key);
					}
				}

				foreach (var module in DependenciesReadOnly)
				{
					module.Value.GetRedundantDependencies(unusedDependencies, visited);
				}

				return unusedDependencies;
			}

			public IDictionary<string, HashSet<string>> GetRedundantReferences(IDictionary<string, HashSet<string>>? unusedReferences = null, HashSet<string>? visited = null)
			{
				unusedReferences ??= new Dictionary<string, HashSet<string>>();
				visited ??= [];

				if (!visited.Add(Name))
				{
					return unusedReferences;
				}

				if (!unusedReferences.ContainsKey(Name))
				{
					unusedReferences[Name] = [];
				}

				foreach (var module in ReferencesReadOnly)
				{
					if (ReferencesReadOnly.Where(otherModule => module.Key != otherModule.Key).Any(otherModule => otherModule.Value.ReferencedBy(module.Value)))
					{
						unusedReferences[Name].Add(module.Key);
					}
				}

				foreach (var module in ReferencesReadOnly)
				{
					module.Value.GetRedundantReferences(unusedReferences, visited);
				}

				return unusedReferences;
			}

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

				if (!_dependencies.Remove(module.Name))
				{
					return;
				}

				module.RemoveReference(this);
			}

			public void RemoveReference(IModule module)
			{
				//same context?
				if (!ReferenceEquals(DataCore, module.DataCore))
				{
					return;
				}

				if (!_references.Remove(module.Name))
				{
					return;
				}

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