using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectQueriesReadOnly
{
	IEnumerable<IReadOnlyProject> GetInternalTopLevelProjects();
	IEnumerable<IReadOnlyProject> GetSubLevelProjects();
	IEnumerable<IReadOnlyProject> GetTopLevelProjects();
}