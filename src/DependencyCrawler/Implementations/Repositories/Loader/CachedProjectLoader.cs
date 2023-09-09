using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Entities.CachedTypes;

namespace DependencyCrawler.Implementations.Repositories.Loader;

internal class CachedProjectLoader : ICachedProjectLoader
{
	private readonly ICachedProjectProvider _cachedProjectProvider;
	private readonly ICachedTypeFactory _cachedTypeFactory;
	private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

	public CachedProjectLoader(ICachedProjectProvider cachedProjectProvider, ICachedTypeFactory cachedTypeFactory,
		IReadOnlyProjectProvider readOnlyProjectProvider)
	{
		_cachedProjectProvider = cachedProjectProvider;
		_cachedTypeFactory = cachedTypeFactory;
		_readOnlyProjectProvider = readOnlyProjectProvider;
	}

	public IList<CachedProject> GetCachedProjects()
	{
		_cachedProjectProvider.Clear();
		var projects = _readOnlyProjectProvider.AllProjectsReadOnly.Values;

		foreach (var project in projects)
		{
			CacheProject(project);
		}

		return _cachedProjectProvider.CachedProjects;
	}

	private void CacheProject(IReadOnlyProject project)
	{
		var _ = _cachedTypeFactory.GetCachedProject(project);
	}
}