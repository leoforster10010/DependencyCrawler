using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ICachedTypeFactory
{
	CachedProject GetCachedProject(IProject project);
}