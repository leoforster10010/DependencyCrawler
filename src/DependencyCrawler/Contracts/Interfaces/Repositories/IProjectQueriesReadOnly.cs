using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectQueriesReadOnly
{
    IDictionary<string, IReadOnlyInternalProject> GetInternalTopLevelProjects();
    IDictionary<string, IReadOnlyProject> GetSubLevelProjects();
    Dictionary<string, IReadOnlyProject> GetTopLevelProjects();
}