using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Entities.CachedTypes;

namespace DependencyCrawler.Implementations.Repositories.Provider;

internal class CachedProjectProvider : ICachedProjectProvider
{
	private readonly IDictionary<string, CachedProject> _cachedProjects = new Dictionary<string, CachedProject>();

	public Guid? GetCachedProjectId(string projectName)
	{
		return _cachedProjects.TryGetValue(projectName, out var project)
			? project.Id
			: null;
	}

	public IList<CachedProject> CachedProjects => _cachedProjects.Values.ToList();

	public Guid? GetCachedNamespaceId(string namespaceName)
	{
		var cachedProject =
			_cachedProjects.Values.FirstOrDefault(x => x.Namespaces.Any(y => y.Name == namespaceName));

		return cachedProject?.Namespaces.First(x => x.Name == namespaceName).Id;
	}

	public void Clear()
	{
		_cachedProjects.Clear();
	}

	public void AddCachedProject(CachedProject cachedProject)
	{
		_cachedProjects.TryAdd(cachedProject.Name, cachedProject);
	}
}