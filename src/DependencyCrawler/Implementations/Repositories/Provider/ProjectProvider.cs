using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ProjectProvider : IProjectProvider
{
    public IDictionary<string, UnresolvedProject> UnresolvedProjects { get; } =
        new Dictionary<string, UnresolvedProject>();

    public IDictionary<string, InternalProject> InternalProjects { get; } = new Dictionary<string, InternalProject>();
    public IDictionary<string, ExternalProject> ExternalProjects { get; } = new Dictionary<string, ExternalProject>();

    public IDictionary<string, IProject> AllProjects
    {
        get
        {
            var allProjects = new Dictionary<string, IProject>();
            foreach (var internalProject in InternalProjects)
            {
                allProjects.TryAdd(internalProject.Key, internalProject.Value);
            }

            foreach (var externalProject in ExternalProjects)
            {
                allProjects.TryAdd(externalProject.Key, externalProject.Value);
            }

            foreach (var unresolvedProject in UnresolvedProjects)
            {
                allProjects.TryAdd(unresolvedProject.Key, unresolvedProject.Value);
            }

            return allProjects;
        }
    }

    public void AddInternalProject(InternalProject internalProject)
    {
        InternalProjects.TryAdd(internalProject.Name, internalProject);
    }

    public void AddExternalProject(ExternalProject externalProject)
    {
        ExternalProjects.TryAdd(externalProject.Name, externalProject);
    }

    public void AddUnresolvedProject(UnresolvedProject unresolvedProject)
    {
        UnresolvedProjects.TryAdd(unresolvedProject.Name, unresolvedProject);
    }

    public void Clear()
    {
        ExternalProjects.Clear();
        InternalProjects.Clear();
        UnresolvedProjects.Clear();
    }

    public IReadOnlyDictionary<string, IReadOnlyInternalProject> InternalProjectsReadOnly =>
        InternalProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyInternalProject);

    public IReadOnlyDictionary<string, IReadOnlyExternalProject> ExternalProjectsReadOnly =>
        ExternalProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyExternalProject);

    public IReadOnlyDictionary<string, IReadOnlyProject> AllProjectsReadOnly =>
        AllProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyProject);
}