using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class ProjectProvider : IProjectProvider
{
	private readonly ILogger<ProjectProvider> _logger;

	public ProjectProvider(ILogger<ProjectProvider> logger)
	{
		_logger = logger;
	}

	public IDictionary<string, UnresolvedProject> UnresolvedProjects { get; } =
		new Dictionary<string, UnresolvedProject>();

	public IDictionary<string, InternalProject> InternalProjects { get; } = new Dictionary<string, InternalProject>();
	public IDictionary<string, ExternalProject> ExternalProjects { get; } = new Dictionary<string, ExternalProject>();

	public IDictionary<string, IProject> AllProjects
	{
		get
		{
			var allProjects = new Dictionary<string, IProject>();
			foreach (var internalProject in InternalProjects)
			{
				allProjects.TryAdd(internalProject.Key, internalProject.Value);
			}

			foreach (var externalProject in ExternalProjects)
			{
				allProjects.TryAdd(externalProject.Key, externalProject.Value);
			}

			foreach (var unresolvedProject in UnresolvedProjects)
			{
				allProjects.TryAdd(unresolvedProject.Key, unresolvedProject.Value);
			}

			return allProjects;
		}
	}

	public void AddInternalProject(InternalProject internalProject)
	{
		_logger.LogInformation($"Added {internalProject.Name} to InternalProjects");
		InternalProjects.TryAdd(internalProject.Name, internalProject);
	}

	public void AddExternalProject(ExternalProject externalProject)
	{
		_logger.LogInformation($"Added {externalProject.Name} to ExternalProjects");
		ExternalProjects.TryAdd(externalProject.Name, externalProject);
	}

	public void AddUnresolvedProject(UnresolvedProject unresolvedProject)
	{
		_logger.LogWarning($"Added {unresolvedProject.Name} to UnresolvedProjects");
		UnresolvedProjects.TryAdd(unresolvedProject.Name, unresolvedProject);
	}

	public void Clear()
	{
		ExternalProjects.Clear();
		InternalProjects.Clear();
		UnresolvedProjects.Clear();
	}
}