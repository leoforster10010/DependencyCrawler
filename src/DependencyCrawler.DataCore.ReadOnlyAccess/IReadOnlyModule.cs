using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.ReadOnlyAccess;

public interface IReadOnlyModule : IValueModule
{
	IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly { get; }
	int DependencyLayer { get; }
	int ReferenceLayer { get; }
	bool IsTopLevel { get; }
	bool IsSubLevel { get; }

	public bool DependsOn(IReadOnlyModule module)
	{
		if (this == module)
		{
			return true;
		}

		if (DependencyLayer <= module.DependencyLayer)
		{
			return false;
		}

		if (DependenciesReadOnly.ContainsKey(module.Name))
		{
			return true;
		}

		return DependenciesReadOnly.Values.Any(x => x.DependsOn(module));
	}

	public IDictionary<string, IReadOnlyModule> GetAllDependencies()
	{
		return CollectDependencies(this, new Dictionary<string, IReadOnlyModule>());
	}

	private Dictionary<string, IReadOnlyModule> CollectDependencies(IReadOnlyModule module, Dictionary<string, IReadOnlyModule> allDependencies)
	{
		foreach (var dependency in module.DependenciesReadOnly)
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

	public IDictionary<string, IReadOnlyModule> GetAllReferences()
	{
		return CollectReferences(this, new Dictionary<string, IReadOnlyModule>());
	}

	private Dictionary<string, IReadOnlyModule> CollectReferences(IReadOnlyModule module, Dictionary<string, IReadOnlyModule> allReferences)
	{
		foreach (var reference in module.ReferencesReadOnly)
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