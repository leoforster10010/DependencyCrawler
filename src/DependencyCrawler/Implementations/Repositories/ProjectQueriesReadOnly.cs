using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Repositories;

internal class ProjectQueriesReadOnly : IProjectQueriesReadOnly
{
    private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

    public ProjectQueriesReadOnly(IReadOnlyProjectProvider readOnlyProjectProvider)
    {
        _readOnlyProjectProvider = readOnlyProjectProvider;
    }

    public IDictionary<string, IReadOnlyInternalProject> GetInternalTopLevelProjects()
    {
        return _readOnlyProjectProvider.InternalProjectsReadOnly
            .Where(x => !x.Value.DependenciesReadOnly.Any(y =>
                y.Value.UsingReadOnly.ProjectTypeReadOnly is ProjectType.Internal))
            .ToDictionary(p => p.Key, p => p.Value);
    }

    public IDictionary<string, IReadOnlyProject> GetSubLevelProjects()
    {
        return _readOnlyProjectProvider.AllProjectsReadOnly.Where(x =>
                x.Value.ProjectTypeReadOnly is not ProjectType.Unresolved && !x.Value.ReferencesReadOnly.Any())
            .ToDictionary(p => p.Key, p => p.Value);
    }

    public Dictionary<string, IReadOnlyProject> GetTopLevelProjects()
    {
        return _readOnlyProjectProvider.AllProjectsReadOnly.Where(x =>
                x.Value.ProjectTypeReadOnly is not ProjectType.Unresolved && !x.Value.DependenciesReadOnly.Any())
            .ToDictionary(p => p.Key, p => p.Value);
    }
}