using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Implementations.Repositories.Loader;

public class CachedProjectLoader : ICachedProjectLoader
{
	private readonly ICachedProjectProvider _cachedProjectProvider;
	private readonly ICachedTypeFactory _cachedTypeFactory;
	private readonly IProjectLoader _projectLoader;
	private readonly IProjectProvider _projectProvider;

	public CachedProjectLoader(ICachedProjectProvider cachedProjectProvider, ICachedTypeFactory cachedTypeFactory,
		IProjectProvider projectProvider, IProjectLoader projectLoader)
	{
		_cachedProjectProvider = cachedProjectProvider;
		_cachedTypeFactory = cachedTypeFactory;
		_projectProvider = projectProvider;
		_projectLoader = projectLoader;
	}

	public IList<CachedProject> GetCachedProjects()
	{
		var projects = _projectProvider.AllProjects.Values;

		foreach (var project in projects)
		{
			CacheProject(project);
		}

		return _cachedProjectProvider.CachedProjects;
	}

	private void CacheProject(IProject project)
	{
		var cachedProject = _cachedTypeFactory.GetCachedProject(project);
		_cachedProjectProvider.AddCachedProject(cachedProject);
	}
}