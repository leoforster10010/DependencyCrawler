using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Implementations.Models;

namespace DependencyCrawler.Framework.Extensions;

public static class ReadOnlyProjectExtensions
{
	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetRequiredDependenciesReadOnly(
		this IReadOnlyProject project, bool includeUnresolved = false)
	{
		var requiredDependencies = project.UsingDirectivesReadOnly.Values
			.Select(x => x.ReferencedNamespaceReadOnly.ParentProjectReadOnly)
			.Where(x => includeUnresolved || x.ProjectTypeReadOnly is not ProjectType.Unresolved)
			.GroupBy(x => x.Id)
			.ToDictionary(x => x.Key, x => x.First());

		return requiredDependencies;
	}

	public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetRequiredByReadOnly(this IReadOnlyProject project)
	{
		var requiredDependencies = project.NamespacesReadOnly.Values
			.SelectMany(x => x.UsingTypesReadOnly.Values.Select(y => y.ParentProjectReadOnly))
			.GroupBy(x => x.Id)
			.ToDictionary(x => x.Key, x => x.First());

		return requiredDependencies;
	}

	public static double GetAverageDependencyPathDistance(this IReadOnlyProject project)
	{
		var dependencyPathInfos = project.GetDependencyPathInfos();

		if (!dependencyPathInfos.Any())
		{
			return 0;
		}

		return dependencyPathInfos.Select(x => x.AveragePathDistance).Average();
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

	public static DependencyPathInfo GetDependencyPathInfo(this IReadOnlyProject project,
		IReadOnlyProject dependency)
	{
		if (project.DependencyLayerReadOnly == dependency.DependencyLayerReadOnly)
		{
			return new DependencyPathInfo
			{
				PossiblePaths = new List<IReadOnlyList<IReadOnlyProject>>(),
				Project = project,
				Dependency = dependency
			};
		}

		if (project.DependencyLayerReadOnly < dependency.DependencyLayerReadOnly)
		{
			return new DependencyPathInfo
			{
				PossiblePaths = GetDependencyPaths(new List<IReadOnlyProject> { dependency }, project),
				Project = dependency,
				Dependency = project
			};
		}

		return new DependencyPathInfo
		{
			PossiblePaths = GetDependencyPaths(new List<IReadOnlyProject> { project }, dependency),
			Project = project,
			Dependency = dependency
		};
	}

	private static IReadOnlyList<IReadOnlyList<IReadOnlyProject>> GetDependencyPaths(
		IList<IReadOnlyProject> currentPath,
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

	public static IList<DependencyPathInfo> GetDependencyPathInfos(this IReadOnlyProject project)
	{
		var requiredDependencyPathInfos = new List<DependencyPathInfo>();
		var referencingProjects = project.GetRequiredByReadOnly().Values;

		foreach (var requiredDependency in referencingProjects)
		{
			requiredDependencyPathInfos.Add(project.GetDependencyPathInfo(requiredDependency));
		}

		return requiredDependencyPathInfos;
	}
}