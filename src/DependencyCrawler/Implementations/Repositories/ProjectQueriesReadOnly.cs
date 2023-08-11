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

	public IEnumerable<IReadOnlyProject> GetInternalTopLevelProjects()
	{
		var toplevelProjects = _readOnlyProjectProvider.InternalProjectsReadOnly.Values
			.Where(x => !x.DependenciesReadOnly.Any() ||
			            x.DependenciesReadOnly.Values.All(y =>
				            y.UsingReadOnly.ProjectTypeReadOnly == ProjectType.External));

		return toplevelProjects;
	}

	public IEnumerable<IReadOnlyProject> GetSubLevelProjects()
	{
		var projects = _readOnlyProjectProvider.AllProjectsReadOnly;
		return projects.Where(x =>
				projects.All(y => y.Value.DependenciesReadOnly.All(z => z.Value.UsingReadOnly != x.Value)))
			.Select(x => x.Value);
	}

	public IEnumerable<IReadOnlyProject> GetTopLevelProjects()
	{
		var toplevelProjects = _readOnlyProjectProvider.AllProjectsReadOnly.Values
			.Where(x => !x.DependenciesReadOnly.Any());

		return toplevelProjects;
	}
}