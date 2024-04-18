using System.Collections.Concurrent;
using DependencyCrawler.DataCore.ReadOnlyAccess;
using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

public class Module : IReadOnlyModule, IValueModule
{
	public required string Name { get; init; }
	public required IDependencyCrawlerCore DependencyCrawlerCore { get; init; }

	public ConcurrentDictionary<string, Module> DependingOn { get; } = new();
	public ConcurrentDictionary<string, Module> DependencyOf { get; } = new();


	public string NameReadOnly => Name;
	public IReadOnlyDictionary<string, IReadOnlyModule> DependingOnReadOnly => DependingOn.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IReadOnlyModule> DependencyOfReadOnly => DependencyOf.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);


	public string NameValue => Name;
	public HashSet<string> DependingOnValue => DependingOn.Keys.ToHashSet();
	public HashSet<string> DependencyOfValue => DependencyOf.Keys.ToHashSet();

	public void Delete()
	{
		foreach (var dependencyOf in DependencyOf)
		{
			dependencyOf.Value.DependingOn.TryRemove(Name, out _);
		}

		foreach (var dependingOn in DependingOn)
		{
			dependingOn.Value.DependencyOf.TryRemove(Name, out _);
		}

		DependencyCrawlerCore.Modules.TryRemove(Name, out _);
	}

	public void AddDependencyOf(string key)
	{
		if (!DependencyCrawlerCore.Modules.TryGetValue(key, out var dependencyOf))
		{
			return;
		}

		dependencyOf.DependingOn.TryAdd(Name, this);
		DependencyOf.TryAdd(dependencyOf.Name, dependencyOf);
	}

	public void AddDependingOn(string key)
	{
		if (!DependencyCrawlerCore.Modules.TryGetValue(key, out var dependingOn))
		{
			return;
		}

		dependingOn.DependencyOf.TryAdd(Name, this);
		DependingOn.TryAdd(dependingOn.Name, dependingOn);
	}
}