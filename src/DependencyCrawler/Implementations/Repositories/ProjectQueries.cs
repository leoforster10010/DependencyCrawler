using DependencyCrawler.Contracts.Interfaces;
using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Repositories;

public class ProjectQueries : IProjectQueries
{
	private readonly IProjectProvider _projectProvider;

	public ProjectQueries(IProjectProvider projectProvider)
	{
		_projectProvider = projectProvider;
	}

	public IEnumerable<IProject> GetInternalTopLevelProjects()
	{
		var toplevelProjects = _projectProvider.InternalProjects
			.Where(x => !x.Dependencies.Any() ||
			            x.Dependencies.Values.All(y => y.Using.ProjectType == ProjectType.External));

		return toplevelProjects;
	}

	public IEnumerable<IProject> GetSubLevelProjects()
	{
		var projects = _projectProvider.AllProjects;
		return projects.Where(x => projects.All(y => y.Value.Dependencies.All(z => z.Value.Using != x.Value)))
			.Select(x => x.Value);
	}

	public IEnumerable<IProject> GetUsagesForProject(string projectName)
	{
		var projects = _projectProvider.AllProjects;

		return projects.Where(x => x.Value.Dependencies.Any(y => y.Value.Using.Name == projectName))
			.Select(x => x.Value);
	}

	public IEnumerable<IProject> GetTopLevelProjects()
	{
		var toplevelProjects = _projectProvider.AllProjects.Values
			.Where(x => !x.Dependencies.Any());

		return toplevelProjects;
	}
}