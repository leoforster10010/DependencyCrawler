using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.CachedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ICachedTypeFactory
{
	CachedProject GetCachedProject(IProject project);
}