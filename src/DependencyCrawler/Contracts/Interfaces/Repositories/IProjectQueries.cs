using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectQueries
{
	IEnumerable<IReadOnlyProject> GetInternalTopLevelProjects();
	IEnumerable<IReadOnlyProject> GetSubLevelProjects();
	IEnumerable<IReadOnlyProject> GetTopLevelProjects();
}