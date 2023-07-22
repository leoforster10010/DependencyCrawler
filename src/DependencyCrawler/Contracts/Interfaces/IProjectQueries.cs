using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IProjectQueries
{
	IEnumerable<IProject> GetInternalTopLevelProjects();
	IEnumerable<IProject> GetSubLevelProjects();
	IEnumerable<IProject> GetUsagesForProject(string projectName);
	IEnumerable<IProject> GetTopLevelProjects();
}