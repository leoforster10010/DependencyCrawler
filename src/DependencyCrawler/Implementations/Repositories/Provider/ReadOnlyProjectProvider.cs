using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ReadOnlyProjectProvider : IReadOnlyProjectProvider
{
	private readonly IProjectProvider _projectProvider;

	public ReadOnlyProjectProvider(IProjectProvider projectProvider)
	{
		_projectProvider = projectProvider;
	}

	public IReadOnlyDictionary<string, IReadOnlyInternalProject> InternalProjectsReadOnly =>
		_projectProvider.InternalProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyInternalProject);

	public IReadOnlyDictionary<string, IReadOnlyExternalProject> ExternalProjectsReadOnly =>
		_projectProvider.ExternalProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyExternalProject);

	public IReadOnlyDictionary<string, IReadOnlyProject> AllProjectsReadOnly =>
		_projectProvider.AllProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyProject);
}