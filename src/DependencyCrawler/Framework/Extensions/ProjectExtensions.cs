using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Framework.Extensions;

public static class ProjectExtensions
{
    public static bool DependsOn(this IProject project, string projectName)
    {
        return ContainsDependency(project.Dependencies.Values.ToList(), projectName);
    }

    public static bool DependsOn(this IProject project, IProject dependency)
    {
        return ContainsDependency(project.Dependencies.Values.ToList(), dependency.Name);
    }

    private static bool ContainsDependency(IList<IReference> dependencies, string projectName)
    {
        return dependencies.Any(x => x.Using.Name == projectName) ||
               dependencies.Any(dependency =>
                   ContainsDependency(dependency.Using.Dependencies.Values.ToList(), projectName));
    }

    public static IDictionary<string, IProject> GetAllDependenciesRecursive(this IProject project)
    {
        var dependencies = new Dictionary<string, IProject>();

        foreach (var dependency in project.Dependencies)
        {
            dependencies.TryAdd(dependency.Key, dependency.Value.Using);
            var nestedDependencies = dependency.Value.Using.GetAllDependenciesRecursive();

            foreach (var nestedDependency in nestedDependencies)
            {
                dependencies.TryAdd(nestedDependency.Key, nestedDependency.Value);
            }
        }

        return dependencies;
    }

    public static IDictionary<string, IProject> GetAllReferencesRecursive(this IProject project)
    {
        var references = new Dictionary<string, IProject>();

        foreach (var reference in project.ReferencedBy)
        {
            references.TryAdd(reference.Key, reference.Value.UsedBy);
            var nestedReferences = reference.Value.UsedBy.GetAllReferencesRecursive();

            foreach (var nestedReference in nestedReferences)
            {
                references.TryAdd(nestedReference.Key, nestedReference.Value);
            }
        }

        return references;
    }
}