using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ProjectProvider : IProjectProvider, IReadOnlyProjectProvider
{
	private readonly IDictionary<string, ExternalProject> _externalProjects = new Dictionary<string, ExternalProject>();
	private readonly IDictionary<string, InternalProject> _internalProjects = new Dictionary<string, InternalProject>();
	private readonly ILogger<ProjectProvider> _logger;

	private readonly IDictionary<string, UnresolvedProject> _unresolvedProjects =
		new Dictionary<string, UnresolvedProject>();

	public ProjectProvider(ILogger<ProjectProvider> logger)
	{
		_logger = logger;
	}

	public IEnumerable<InternalProject> InternalProjects => _internalProjects.Select(x => x.Value);

	public IEnumerable<ExternalProject> ExternalProjects => _externalProjects.Select(x => x.Value);

	public IDictionary<string, IProject> AllProjects
	{
		get
		{
			var allProjects = new Dictionary<string, IProject>();
			foreach (var internalProject in _internalProjects)
			{
				allProjects.TryAdd(internalProject.Key, internalProject.Value);
			}

			foreach (var externalProject in _externalProjects)
			{
				allProjects.TryAdd(externalProject.Key, externalProject.Value);
			}

			foreach (var unresolvedProject in _unresolvedProjects)
			{
				allProjects.TryAdd(unresolvedProject.Key, unresolvedProject.Value);
			}

			return allProjects;
		}
	}

	public void AddInternalProject(InternalProject internalProject)
	{
		_logger.LogInformation($"Added {internalProject.Name} to InternalProjects");
		_internalProjects.TryAdd(internalProject.Name, internalProject);
	}

	public void AddExternalProject(ExternalProject externalProject)
	{
		_logger.LogInformation($"Added {externalProject.Name} to ExternalProjects");
		_externalProjects.TryAdd(externalProject.Name, externalProject);
	}

	public void AddUnresolvedProject(string name)
	{
		_logger.LogWarning($"Added {name} to UnresolvedProjects");
		_unresolvedProjects.TryAdd(name, new UnresolvedProject
		{
			Name = name
		});
	}

	public void Clear()
	{
		_externalProjects.Clear();
		_internalProjects.Clear();
		_unresolvedProjects.Clear();
	}

	public IReadOnlyList<IReadOnlyProject> InternalProjectsReadOnly => _internalProjects.Select(x => x.Value).ToList();
	public IReadOnlyList<IReadOnlyProject> ExternalProjectsReadOnly => _externalProjects.Select(x => x.Value).ToList();

	public IReadOnlyDictionary<string, IReadOnlyProject> AllProjectsReadOnly =>
		AllProjects.ToDictionary(x => x.Key, x => x.Value as IReadOnlyProject);
}