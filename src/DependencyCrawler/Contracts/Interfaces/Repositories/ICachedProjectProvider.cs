using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ICachedProjectProvider
{
	Guid? GetCachedProjectId(string name);
	void AddCachedProject(CachedProject cachedProject);
	Guid? GetCachedNamespaceId(string namespaceName);
}