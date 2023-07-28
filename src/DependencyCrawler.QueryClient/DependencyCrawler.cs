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

        //Projects x depends on directly
        var directDependencies = _projectProvider.AllProjects["x"].Dependencies.Values.Select(x => x.Using);
        //Projects x depends on directly or indirect
        var allDependencies = _projectProvider.AllProjects["x"].GetAllDependenciesRecursive();

        //all projects depending on x directly
        var referencedBy = _projectProvider.AllProjects["x"].ReferencedBy.Values.Select(x => x.UsedBy);
        //all projects depending on x directly or indirect
        var dependingProjects = _projectProvider.AllProjects["x"].GetAllReferencesRecursive();

        //internal projects depending only on external projects
        var internalTopLevelProjects = _projectQueries.GetInternalTopLevelProjects();
        //projects depending on no other projects
        var topLevelProjects = _projectQueries.GetTopLevelProjects();
        //projects no other project depends on
        var subLevelProjects = _projectQueries.GetSubLevelProjects();

        //check if a depends directly on b
        var dependsDirectly = _projectProvider.AllProjects["a"].Dependencies.Any(x => x.Value.Using.Name == "b");
        //check if a depends on b
        var dependsRecursive = _projectProvider.AllProjects["a"].DependsOn("b");
        var dependsRecursiveOverload = _projectProvider.AllProjects["a"].DependsOn(_projectProvider.AllProjects["b"]);
    }
}