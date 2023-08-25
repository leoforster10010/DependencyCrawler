using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Framework.Extensions;

public static class ReadOnlyProjectExtensions
{
    public static bool DependsOnReadOnly(this IReadOnlyProject project, IReadOnlyProject dependency,
        IDictionary<Guid, IReadOnlyReference>? excludedReferences = null,
        IDictionary<Guid, IReadOnlyReference>? includedReferences = null)
    {
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

        if (dependencies.Values.Any(x => x.UsingReadOnly == dependency))
        {
            return true;
        }


        return dependencies.Values.Any(x => x.UsingReadOnly.DependsOnReadOnly(dependency));
    }

    public static IReadOnlyDictionary<Guid, IReadOnlyProject> GetAllDependenciesRecursive(
        this IReadOnlyProject project, bool internalOnly = false)
    {
        var dependencies = new Dictionary<Guid, IReadOnlyProject>();
        var dependenciesToSearch = project.DependenciesReadOnly.Where(x =>
            !internalOnly || x.Value.UsingReadOnly.ProjectTypeReadOnly is ProjectType.Internal);

        foreach (var dependency in dependenciesToSearch)
        {
            dependencies.TryAdd(dependency.Key, dependency.Value.UsingReadOnly);
            var nestedDependencies = dependency.Value.UsingReadOnly.GetAllDependenciesRecursive(internalOnly);

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

        foreach (var reference in project.ReferencesReadOnly)
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