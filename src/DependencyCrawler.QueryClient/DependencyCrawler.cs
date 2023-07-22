using DependencyCrawler.Contracts.Interfaces;

namespace DependencyCrawler.QueryClient;

public class DependencyCrawler : IDependencyCrawler
{
	private readonly IProjectLoader _projectLoader;
	private readonly IProjectProvider _projectProvider;

	public DependencyCrawler(IProjectProvider projectProvider, IProjectLoader projectLoader)
	{
		_projectProvider = projectProvider;
		_projectLoader = projectLoader;
	}

	public void Run()
	{
		_projectLoader.LoadAllProjects();
		//var project = _projectLoader.LoadProjectByName("");
		var allProjects = _projectProvider.AllProjects;
	}
}