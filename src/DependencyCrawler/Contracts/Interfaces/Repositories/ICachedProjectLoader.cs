using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ICachedProjectLoader
{
	IList<CachedProject> GetCachedProjects();
}