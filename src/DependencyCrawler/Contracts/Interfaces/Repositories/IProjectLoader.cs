using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IProjectLoader
{
	void LoadAllProjects();
	IProject LoadProjectByName(string name);
	void LoadProjectsFromCache(Cache cache);
}