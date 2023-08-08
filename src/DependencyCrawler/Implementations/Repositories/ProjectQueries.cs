using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Repositories;

internal class ProjectQueries : IProjectQueries
{
	private readonly IProjectProvider _projectProvider;

	public ProjectQueries(IProjectProvider projectProvider)
	{
		_projectProvider = projectProvider;
	}

	public IEnumerable<IReadOnlyProject> GetInternalTopLevelProjects()
	{
		var toplevelProjects = _projectProvider.InternalProjects
			.Where(x => !x.Dependencies.Any() ||
			            x.Dependencies.Values.All(y => y.Using.ProjectType == ProjectType.External));

		return toplevelProjects;
	}

	public IEnumerable<IReadOnlyProject> GetSubLevelProjects()
	{
		var projects = _projectProvider.AllProjects;
		return projects.Where(x => projects.All(y => y.Value.Dependencies.All(z => z.Value.Using != x.Value)))
			.Select(x => x.Value);
	}

	public IEnumerable<IReadOnlyProject> GetTopLevelProjects()
	{
		var toplevelProjects = _projectProvider.AllProjects.Values
			.Where(x => !x.Dependencies.Any());

		return toplevelProjects;
	}
}