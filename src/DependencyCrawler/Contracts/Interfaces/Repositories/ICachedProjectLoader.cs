using DependencyCrawler.Data.Contracts.Entities.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ICachedProjectLoader
{
	IList<CachedProject> GetCachedProjects();
}