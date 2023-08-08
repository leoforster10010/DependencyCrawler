using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ICachedProjectLoader
{
	IList<CachedProject> GetCachedProjects();
}