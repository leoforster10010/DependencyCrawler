using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Framework.Extensions;

public static class ReadOnlyProjectExtensions
{
	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetRequiredDependenciesReadOnly(
		this IReadOnlyProject project, bool includeUnresolved = false)
	{
		var requiredDependencies = project.UsingDirectivesReadOnly.Values
			.Select(x => x.ReferencedNamespaceReadOnly.ParentProjectReadOnly)
			.Where(x => includeUnresolved || x.ProjectTypeReadOnly is not ProjectType.Unresolved)
			.ToDictionary(x => x.Id, x => x);

		return requiredDependencies;
	}

	public static bool Requires(this IReadOnlyProject project, IReadOnlyProject dependency,
		bool includeUnresolved = false)
	{
		return project.UsingDirectivesReadOnly.Values
			.Where(x => includeUnresolved ||
			            x.ReferencedNamespaceReadOnly.ParentProjectReadOnly.ProjectTypeReadOnly is not ProjectType
				            .Unresolved)
			.Any(x => x.ReferencedNamespaceReadOnly.ParentProjectReadOnly == dependency);
	}

	public static bool DependsOn(this IReadOnlyProject project, IReadOnlyProject dependency,
		IDictionary<Guid, IReadOnlyReference>? excludedReferences = null)
	{
		if (project == dependency)
		{
			return true;
		}

		if (project.DependencyLayerReadOnly <= dependency.DependencyLayerReadOnly)
		{
			return false;
		}

		var dependencies = project.DependenciesReadOnly;
		if (excludedReferences is not null)
		{
			dependencies = dependencies.Where(x => !excludedReferences.ContainsKey(x.Key))
				.ToDictionary(x => x.Key, x => x.Value);
		}

		return dependencies.Values.Any(x => x.UsingReadOnly.DependsOn(dependency));
	}

	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetAllDependenciesReadOnlyRecursive(
		this IReadOnlyProject project, bool internalOnly = false)
	{
		var dependencies = new Dictionary<Guid, IReadOnlyProject>();
		var dependenciesToSearch = project.DependenciesReadOnly.Where(x =>
			!internalOnly || x.Value.UsingReadOnly.ProjectTypeReadOnly is ProjectType.Internal);

		foreach (var dependency in dependenciesToSearch)
		{
			dependencies.TryAdd(dependency.Value.UsingReadOnly.Id, dependency.Value.UsingReadOnly);
			var nestedDependencies = dependency.Value.UsingReadOnly.GetAllDependenciesReadOnlyRecursive(internalOnly);

			foreach (var nestedDependency in nestedDependencies)
			{
				dependencies.TryAdd(nestedDependency.Key, nestedDependency.Value);
			}
		}

		return dependencies;
	}

	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetAllReferencesReadOnlyRecursive(
		this IReadOnlyProject project)
	{
		var references = new Dictionary<Guid, IReadOnlyProject>();

		foreach (var reference in project.ReferencesReadOnly)
		{
			references.TryAdd(reference.Value.UsedByReadOnly.Id, reference.Value.UsedByReadOnly);
			var nestedReferences = reference.Value.UsedByReadOnly.GetAllReferencesReadOnlyRecursive();

			foreach (var nestedReference in nestedReferences)
			{
				references.TryAdd(nestedReference.Key, nestedReference.Value);
			}
		}

		return references;
	}

	public static IReadOnlyList<IReadOnlyList<IReadOnlyProject>> GetDependencyPaths(this IReadOnlyProject project,
		IReadOnlyProject dependency)
	{
		if (project.DependencyLayerReadOnly == dependency.DependencyLayerReadOnly)
		{
			return new List<IReadOnlyList<IReadOnlyProject>>();
		}

		return project.DependencyLayerReadOnly < dependency.DependencyLayerReadOnly
			? GetDependencyPaths(new List<IReadOnlyProject> { dependency }, project).AsReadOnly()
			: GetDependencyPaths(new List<IReadOnlyProject> { project }, dependency).AsReadOnly();
	}

	private static IList<IReadOnlyList<IReadOnlyProject>> GetDependencyPaths(IList<IReadOnlyProject> currentPath,
		IReadOnlyProject dependency)
	{
		var resultPaths = new List<IReadOnlyList<IReadOnlyProject>>();
		currentPath = currentPath.ToList();

		if (currentPath.Last().DependencyLayerReadOnly <= dependency.DependencyLayerReadOnly)
		{
			return resultPaths;
		}

		foreach (var readOnlyReference in currentPath.Last().DependenciesReadOnly.Values)
		{
			if (!readOnlyReference.UsingReadOnly.DependsOn(dependency))
			{
				continue;
			}

			var newPath = currentPath.ToList();
			if (readOnlyReference.UsingReadOnly == dependency)
			{
				newPath.Add(readOnlyReference.UsingReadOnly);
				resultPaths.Add(newPath);
			}
			else
			{
				currentPath.Add(readOnlyReference.UsingReadOnly);
				resultPaths.AddRange(GetDependencyPaths(currentPath, dependency));
			}
		}

		return resultPaths;
	}
}