using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Framework.Extensions;

public static class ReadOnlyProjectExtensions
{
	public static bool DependsOn(this IReadOnlyProject project, string projectName)
	{
		return ContainsDependency(project.DependenciesReadOnly.Values.ToList(), projectName);
	}

	public static bool DependsOn(this IReadOnlyProject project, IReadOnlyProject dependency)
	{
		return ContainsDependency(project.DependenciesReadOnly.Values.ToList(), dependency.NameReadOnly);
	}

	private static bool ContainsDependency(IList<IReadOnlyReference> dependencies, string projectName)
	{
		return dependencies.Any(x => x.UsingReadOnly.NameReadOnly == projectName) ||
		       dependencies.Any(dependency =>
			       ContainsDependency(dependency.UsingReadOnly.DependenciesReadOnly.Values.ToList(), projectName));
	}

	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetAllDependenciesRecursive(
		this IReadOnlyProject project)
	{
		var dependencies = new Dictionary<Guid, IReadOnlyProject>();

		foreach (var dependency in project.DependenciesReadOnly)
		{
			dependencies.TryAdd(dependency.Key, dependency.Value.UsingReadOnly);
			var nestedDependencies = dependency.Value.UsingReadOnly.GetAllDependenciesRecursive();

			foreach (var nestedDependency in nestedDependencies)
			{
				dependencies.TryAdd(nestedDependency.Key, nestedDependency.Value);
			}
		}

		return dependencies;
	}

	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetAllReferencesRecursive(this IReadOnlyProject project)
	{
		var references = new Dictionary<Guid, IReadOnlyProject>();

		foreach (var reference in project.ReferencedByReadOnly)
		{
			references.TryAdd(reference.Key, reference.Value.UsedByReadOnly);
			var nestedReferences = reference.Value.UsedByReadOnly.GetAllReferencesRecursive();

			foreach (var nestedReference in nestedReferences)
			{
				references.TryAdd(nestedReference.Key, nestedReference.Value);
			}
		}

		return references;
	}
}