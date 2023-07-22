using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IProjectLoader
{
	void LoadAllProjects();
	IProject LoadProjectByName(string name);
}