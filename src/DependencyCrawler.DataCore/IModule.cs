using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.DataCore;

public interface IModule : IReadOnlyModule, IEntity
{
	IReadOnlyDictionary<string, IModule> Dependencies { get; }
	IReadOnlyDictionary<string, IModule> References { get; }
	void AddDependency(IModule module);
	void AddReference(IModule module);
	void RemoveDependency(IModule module);
	void RemoveReference(IModule module);

	public new IDictionary<string, IModule> GetAllDependencies()
	{
		return CollectDependencies(this, new Dictionary<string, IModule>());
	}

	private Dictionary<string, IModule> CollectDependencies(IModule module, Dictionary<string, IModule> allDependencies)
	{
		foreach (var dependency in module.Dependencies)
		{
			if (allDependencies.ContainsKey(dependency.Key))
			{
				continue;
			}

			allDependencies[dependency.Key] = dependency.Value;
			CollectDependencies(dependency.Value, allDependencies);
		}

		return allDependencies;
	}

	public new IDictionary<string, IModule> GetAllReferences()
	{
		return CollectReferences(this, new Dictionary<string, IModule>());
	}

	private Dictionary<string, IModule> CollectReferences(IModule module, Dictionary<string, IModule> allReferences)
	{
		foreach (var reference in module.References)
		{
			if (allReferences.ContainsKey(reference.Key))
			{
				continue;
			}

			allReferences[reference.Key] = reference.Value;
			CollectReferences(reference.Value, allReferences);
		}

		return allReferences;
	}
}