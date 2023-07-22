using DependencyCrawler.Contracts.Interfaces;
using DependencyCrawler.Framework.Extensions;

namespace DependencyCrawler.QueryClient;

public class DependencyCrawler : IDependencyCrawler
{
	private readonly IProjectLoader _projectLoader;
	private readonly IProjectProvider _projectProvider;
	private readonly IProjectQueries _projectQueries;

	public DependencyCrawler(IProjectProvider projectProvider, IProjectLoader projectLoader,
		IProjectQueries projectQueries)
	{
		_projectProvider = projectProvider;
		_projectLoader = projectLoader;
		_projectQueries = projectQueries;
	}

	public void Run()
	{
		_projectLoader.LoadAllProjects();

		var allProjects = _projectProvider.AllProjects;
		var dependingProjects = _projectProvider.AllProjects.Where(x => x.Value.DependsOn(""));
		var allDependencies = _projectProvider.AllProjects[""].GetAllDependenciesRecursive();
		var usages = _projectQueries.GetUsagesForProject("");
		var internalTopLevelProjects = _projectQueries.GetInternalTopLevelProjects();
		var subLevelProjects = _projectQueries.GetSubLevelProjects();
	}
}