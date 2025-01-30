using System.Collections.Concurrent;
using DependencyCrawler.Data.Contracts;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

public class Module : IReadOnlyModule, IValueModule
{
	public required IDataCore DataCore { get; init; }
	public ConcurrentDictionary<string, Module> Dependencies { get; } = new();
	public ConcurrentDictionary<string, Module> References { get; } = new();
	public required string Name { get; init; }
	public required ModuleType Type { get; init; }
	public int DependencyLayer => Dependencies.IsEmpty ? 0 : 1 + Dependencies.Max(x => x.Value.DependencyLayer);
	public int ReferenceLayer => References.IsEmpty ? 0 : 1 + References.Max(x => x.Value.ReferenceLayer);
	public bool IsTopLevel => Dependencies.IsEmpty;
	public bool IsSubLevel => References.IsEmpty;


	public IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly => Dependencies.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly => References.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);


	public HashSet<string> DependenciesValue => Dependencies.Keys.ToHashSet(); //ToDo check if new instance
	public HashSet<string> ReferencesValue => References.Keys.ToHashSet();


	public void Delete()
	{
		foreach (var reference in References)
		{
			reference.Value.Dependencies.TryRemove(Name, out _);
		}

		foreach (var dependency in Dependencies)
		{
			dependency.Value.References.TryRemove(Name, out _);
		}

		DataCore.Modules.TryRemove(Name, out _);
	}

	public void AddReference(string key)
	{
		if (!DataCore.Modules.TryGetValue(key, out var reference))
		{
			return;
		}

		reference.Dependencies.TryAdd(Name, this);
		References.TryAdd(reference.Name, reference);
	}

	public void AddDependency(string key)
	{
		if (!DataCore.Modules.TryGetValue(key, out var dependency))
		{
			return;
		}

		dependency.References.TryAdd(Name, this);
		Dependencies.TryAdd(dependency.Name, dependency);
	}

	public IDictionary<string, Module> GetIndirectDependencies()
	{
		var indirectDependencies = Dependencies.ToDictionary();

		foreach (var dependency in Dependencies.Values)
		{
			foreach (var indirectDependency in dependency.GetIndirectDependencies())
			{
				indirectDependencies.TryAdd(indirectDependency.Key, indirectDependency.Value);
			}
		}

		return indirectDependencies;
	}

	public IDictionary<string, Module> GetIndirectReferences()
	{
		var indirectReferences = References.ToDictionary();

		foreach (var reference in References.Values)
		{
			foreach (var indirectReference in reference.GetIndirectReferences())
			{
				indirectReferences.TryAdd(indirectReference.Key, indirectReference.Value);
			}
		}

		return indirectReferences;
	}

	public bool DependsOn(Module module)
	{
		return Dependencies.ContainsKey(module.Name) || Dependencies.Any(x => x.Value.DependsOn(module));
	}

	public bool ReferencedBy(Module module)
	{
		return References.ContainsKey(module.Name) || References.Any(x => x.Value.ReferencedBy(module));
	}
}