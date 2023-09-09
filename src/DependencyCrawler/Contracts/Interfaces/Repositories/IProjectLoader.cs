using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectLoader
{
	void LoadAllProjects();
	void LoadProjectsFromCache(Cache cache);
}