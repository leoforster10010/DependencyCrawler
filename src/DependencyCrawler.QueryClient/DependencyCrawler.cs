using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;

namespace DependencyCrawler.QueryClient;

public class DependencyCrawler : IDependencyCrawler
{
	private readonly IProjectLoader _projectLoader;
	private readonly IProjectQueriesReadOnly _projectQueries;
	private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

	public DependencyCrawler(IReadOnlyProjectProvider readOnlyProjectProvider, IProjectLoader projectLoader,
		IProjectQueriesReadOnly projectQueries)
	{
		_readOnlyProjectProvider = readOnlyProjectProvider;
		_projectLoader = projectLoader;
		_projectQueries = projectQueries;
	}

	public void Run()
	{
		_projectLoader.LoadAllProjects();

		var allProjects = _readOnlyProjectProvider.AllProjectsReadOnly;

		//Projects x depends on directly
		var directDependencies = _readOnlyProjectProvider.AllProjectsReadOnly["x"].DependenciesReadOnly.Values
			.Select(x => x.UsingReadOnly);
		//Projects x depends on directly or indirect
		var allDependencies = _readOnlyProjectProvider.AllProjectsReadOnly["x"].GetAllDependenciesRecursive();

		//all projects depending on x directly
		var referencedBy = _readOnlyProjectProvider.AllProjectsReadOnly["x"].ReferencedByReadOnly.Values
			.Select(x => x.UsedByReadOnly);
		//all projects depending on x directly or indirect
		var dependingProjects = _readOnlyProjectProvider.AllProjectsReadOnly["x"].GetAllReferencesRecursive();

		//internal projects depending only on external projects
		var internalTopLevelProjects = _projectQueries.GetInternalTopLevelProjects();
		//projects depending on no other projects
		var topLevelProjects = _projectQueries.GetTopLevelProjects();
		//projects no other project depends on
		var subLevelProjects = _projectQueries.GetSubLevelProjects();

		//check if a depends directly on b
		var dependsDirectly = _readOnlyProjectProvider.AllProjectsReadOnly["a"].DependenciesReadOnly
			.Any(x => x.Value.UsingReadOnly.NameReadOnly is "b");
		//check if a depends on b
		var dependsRecursive = _readOnlyProjectProvider.AllProjectsReadOnly["a"].DependsOn("b");
		var dependsRecursiveOverload = _readOnlyProjectProvider.AllProjectsReadOnly["a"]
			.DependsOn(_readOnlyProjectProvider.AllProjectsReadOnly["b"]);
	}
}