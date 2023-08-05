using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectQueries
{
	IEnumerable<IProject> GetInternalTopLevelProjects();
	IEnumerable<IProject> GetSubLevelProjects();
	IEnumerable<IProject> GetTopLevelProjects();
}