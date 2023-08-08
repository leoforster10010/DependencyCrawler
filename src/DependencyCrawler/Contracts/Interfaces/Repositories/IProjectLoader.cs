using DependencyCrawler.Implementations.Models;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectLoader
{
	void LoadAllProjects();
	void LoadProjectsFromCache(Cache cache);
}