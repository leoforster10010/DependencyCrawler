using DependencyCrawler.Data.Contracts.Entities.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ICachedProjectProvider
{
	IList<CachedProject> CachedProjects { get; }
	Guid? GetCachedProjectId(string name);
	void AddCachedProject(CachedProject cachedProject);
	Guid? GetCachedNamespaceId(string namespaceName);
	void Clear();
}