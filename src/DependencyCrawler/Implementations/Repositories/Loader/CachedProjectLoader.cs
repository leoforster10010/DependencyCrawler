using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;

namespace DependencyCrawler.Implementations.Repositories.Loader;

public class CachedProjectLoader : ICachedProjectLoader
{
	private readonly ICachedProjectProvider _cachedProjectProvider;
	private readonly ICachedTypeFactory _cachedTypeFactory;
	private readonly IProjectProvider _projectProvider;

	public CachedProjectLoader(ICachedProjectProvider cachedProjectProvider, ICachedTypeFactory cachedTypeFactory,
		IProjectProvider projectProvider)
	{
		_cachedProjectProvider = cachedProjectProvider;
		_cachedTypeFactory = cachedTypeFactory;
		_projectProvider = projectProvider;
	}

	public void CacheLinkedProjects()
	{
		var projects = _projectProvider.AllProjects.Values;

		foreach (var project in projects)
		{
			CacheProject(project);
		}
	}

	private void CacheProject(IProject project)
	{
		var cachedProject = _cachedTypeFactory.GetCachedProject(project);
		_cachedProjectProvider.AddCachedProject(cachedProject);
	}
}