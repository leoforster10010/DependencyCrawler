using DependencyCrawler.DataCore.DataAccess;
using DependencyCrawler.DataCore.ReadOnlyAccess;

namespace DependencyCrawler.QueryClient;

internal class QueryClient(
	IReadOnlyDataAccess readOnlyDataAccess,
	IDataDiscoveryProvider dataDiscoveryProvider) : IDependencyCrawler
{
	public void Run()
	{
		dataDiscoveryProvider.DataDiscoveries.First().Value.Load();

		//var allProjects = readOnlyProjectProvider.AllProjectsReadOnly;
		var allModules = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly;

		//Projects x depends on directly
		var directDependencies = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["x"].DependenciesReadOnly.Values;

		//Projects x depends on directly or indirect
		var allDependencies = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["x"].GetAllDependenciesReadOnly();

		//all projects depending on x directly
		var referencedBy = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["x"].ReferencesReadOnly.Values;

		//all projects depending on x directly or indirect
		var dependingProjects = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["x"].GetAllReferencesReadOnly();

		//projects depending on no other projects
		var topLevelProjects = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly.Where(x => x.Value.IsTopLevel);
		//projects no other project depends on
		var subLevelProjects = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly.Where(x => x.Value.IsSubLevel);

		//check if a depends directly on b
		var dependsDirectly = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].DependenciesReadOnly.ContainsKey("b");

		//check if a depends on b
		var dependsRecursive = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].DependsOn(readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["b"]);

		//check if a is referenced directly by b
		var referencesDirectly = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].ReferencesReadOnly.ContainsKey("b");

		//check if a is referenced by b
		var referencesRecursive = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].ReferencedBy(readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["b"]);

		//get redundant Dependencies
		var redundantDependencies = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].GetRedundantDependencies();

		//get redundant References
		var redundantReferences = readOnlyDataAccess.ActiveCoreReadOnly.ModulesReadOnly["a"].GetRedundantReferences();
	}
}