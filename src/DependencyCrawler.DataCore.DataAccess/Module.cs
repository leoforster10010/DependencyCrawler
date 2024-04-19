using System.Collections.Concurrent;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

public class Module : IReadOnlyModule, IValueModule
{
	public required string Name { get; init; }
	public required IDependencyCrawlerCore DependencyCrawlerCore { get; init; }

	public ConcurrentDictionary<string, Module> Dependencies { get; } = new();
	public ConcurrentDictionary<string, Module> References { get; } = new();
	public IReadOnlyDictionary<string, IReadOnlyModule> ReferencesReadOnly => References.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);


	public string NameReadOnly => Name;
	public IReadOnlyDictionary<string, IReadOnlyModule> DependenciesReadOnly => Dependencies.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public HashSet<string> ReferencesValue => References.Keys.ToHashSet();


	public string NameValue => Name;
	public HashSet<string> DependenciesValue => Dependencies.Keys.ToHashSet();

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

		DependencyCrawlerCore.Modules.TryRemove(Name, out _);
	}

	public void AddReference(string key)
	{
		if (!DependencyCrawlerCore.Modules.TryGetValue(key, out var reference))
		{
			return;
		}

		reference.Dependencies.TryAdd(Name, this);
		References.TryAdd(reference.Name, reference);
	}

	public void AddDependency(string key)
	{
		if (!DependencyCrawlerCore.Modules.TryGetValue(key, out var dependency))
		{
			return;
		}

		dependency.References.TryAdd(Name, this);
		Dependencies.TryAdd(dependency.Name, dependency);
	}
}