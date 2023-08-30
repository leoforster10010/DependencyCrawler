using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.ConsoleClient;

public class ConsoleClient : IConsoleClient
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ICacheManager _cacheManager;
    private readonly IEnumerable<Command> _commands;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<ConsoleClient> _logger;
    private readonly IProjectLoader _projectLoader;
    private readonly IProjectQueriesReadOnly _projectQueries;
    private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

    public ConsoleClient(IReadOnlyProjectProvider readOnlyProjectProvider, IProjectLoader projectLoader,
        IProjectQueriesReadOnly projectQueries, ICacheManager cacheManager, ILogger<ConsoleClient> logger,
        IHostApplicationLifetime applicationLifetime, IEvaluationRepository evaluationRepository)
    {
        _readOnlyProjectProvider = readOnlyProjectProvider;
        _projectLoader = projectLoader;
        _projectQueries = projectQueries;
        _cacheManager = cacheManager;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _evaluationRepository = evaluationRepository;

        _commands = new List<Command>
        {
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "exit",
                    "quit",
                    "q"
                },
                Action = Exit
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "evaluate",
                    "eval"
                },
                Action = Evaluate
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "load files",
                    "load f",
                    "l files",
                    "l f"
                },
                Action = LoadFiles
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "unresolvedUsingDirectives",
                    "uud"
                },
                Action = ListUnresolvedUsingDirectives
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "cache save",
                    "cache s",
                    "c save",
                    "c s"
                },
                Action = SaveCache
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "cache list",
                    "cache ls",
                    "c list",
                    "c ls"
                },
                Action = ListCaches
            },
            new()
            {
                RequiredParameters = 1,
                CommandStrings = new List<string>
                {
                    "cache delete",
                    "cache d",
                    "c delete",
                    "c d"
                },
                Action = DeleteCache
            },
            new()
            {
                RequiredParameters = 1,
                CommandStrings = new List<string>
                {
                    "cache activate",
                    "cache a",
                    "c activate",
                    "c a"
                },
                Action = ActivateCache
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "cache reload",
                    "cache r",
                    "c reload",
                    "c r"
                },
                Action = ReloadCaches
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "projects list",
                    "projects ls",
                    "p list",
                    "p ls"
                },
                Action = ListProjects
            },
            new()
            {
                RequiredParameters = 1,
                CommandStrings = new List<string>
                {
                    "project info",
                    "project i",
                    "p info",
                    "p i"
                },
                Action = ShowProjectInfo
            }
        };
    }

    public void Run(CancellationToken cancellationToken)
    {
        //Load from cache or files
        Load();

        //Idle for input
        while (!cancellationToken.IsCancellationRequested)
        {
            ProcessInput();
        }


        ////Run command


        //_projectLoader.LoadAllProjects();

        //var allProjects = _readOnlyProjectProvider.AllProjectsReadOnly.Values.SelectMany(x =>
        //		x.UsingDirectivesReadOnly.Where(y => y.Value.StateReadOnly is TypeUsingDirectiveState.Unresolved)
        //			.ToList())
        //	.ToList();

        ////Projects x depends on directly
        //var directDependencies = _readOnlyProjectProvider.AllProjectsReadOnly["x"].DependenciesReadOnly.Values
        //	.Select(x => x.UsingReadOnly);
        ////Projects x depends on directly or indirect
        //var allDependencies = _readOnlyProjectProvider.AllProjectsReadOnly["x"].GetAllDependenciesRecursive();

        ////all projects depending on x directly
        //var referencedBy = _readOnlyProjectProvider.AllProjectsReadOnly["x"].ReferencedByReadOnly.Values
        //	.Select(x => x.UsedByReadOnly);
        ////all projects depending on x directly or indirect
        //var dependingProjects = _readOnlyProjectProvider.AllProjectsReadOnly["x"].GetAllReferencesRecursive();

        ////internal projects depending only on external projects
        //var internalTopLevelProjects = _projectQueries.GetInternalTopLevelProjects();
        ////projects depending on no other projects
        //var topLevelProjects = _projectQueries.GetTopLevelProjects();
        ////projects no other project depends on
        //var subLevelProjects = _projectQueries.GetSubLevelProjects();

        ////check if a depends directly on b
        //var dependsDirectly = _readOnlyProjectProvider.AllProjectsReadOnly["a"].DependenciesReadOnly
        //	.Any(x => x.Value.UsingReadOnly.NameReadOnly is "b");
        ////check if a depends on b
        //var dependsRecursive = _readOnlyProjectProvider.AllProjectsReadOnly["a"].DependsOn("b");
        //var dependsRecursiveOverload = _readOnlyProjectProvider.AllProjectsReadOnly["a"]
        //	.DependsOn(_readOnlyProjectProvider.AllProjectsReadOnly["b"]);
    }

    private void Evaluate(IList<string> obj)
    {
        var unusedDependencies = _evaluationRepository.GetUnusedDependencies();
        var alreadyReferencedDependencies = _evaluationRepository.GetAlreadyReferenced();

        Console.WriteLine("Unused Dependencies:");
        foreach (var unusedDependency in unusedDependencies)
        {
            Console.WriteLine(
                $"{unusedDependency.Value.UsedByReadOnly.NameReadOnly} -> {unusedDependency.Value.UsingReadOnly.NameReadOnly}");
        }

        Console.WriteLine("");
        Console.WriteLine("");
        Console.WriteLine("Already referenced Dependencies:");
        foreach (var alreadyReferencedDependency in alreadyReferencedDependencies)
        {
            Console.WriteLine(
                $"{alreadyReferencedDependency.Value.UsedByReadOnly.NameReadOnly} -> {alreadyReferencedDependency.Value.UsingReadOnly.NameReadOnly}");
        }
    }

    private void ListUnresolvedUsingDirectives(IList<string> parameters)
    {
        var unlinkedUsingDirectives = _readOnlyProjectProvider.InternalProjectsReadOnly.SelectMany(x =>
                x.Value.NamespacesReadOnly.Values.SelectMany(y =>
                    y.NamespaceTypesReadOnly.Values.SelectMany(z =>
                        z.UsingDirectivesReadOnly.Values)))
            .Where(x => x.StateReadOnly == TypeUsingDirectiveState.Unresolved)
            .DistinctBy(x => x.NameReadOnly)
            .OrderBy(x => x.NameReadOnly)
            .ToList();

        foreach (var unlinkedUsingDirective in unlinkedUsingDirectives)
        {
            Console.WriteLine($"{unlinkedUsingDirective.NameReadOnly}");
        }
    }

    private void LoadFiles(IList<string> parameters)
    {
        _projectLoader.LoadAllProjects();
    }

    private void ShowProjectInfo(IList<string> parameters)
    {
        var project =
            _readOnlyProjectProvider.AllProjectsReadOnly.Values.FirstOrDefault(x =>
                x.NameReadOnly.ToLower() == parameters.First());
        if (project is null)
        {
            _logger.LogWarning($"Project not found: {parameters.First()}");
            return;
        }

        Console.WriteLine($"{project.NameReadOnly}");
        Console.WriteLine($"Type: {project.ProjectTypeReadOnly}");
        Console.WriteLine($"ReferenceLayer: {project.ReferenceLayerReadOnly}");
        Console.WriteLine($"DependencyLayer: {project.DependencyLayerReadOnly}");
        Console.WriteLine($"Internal DependencyLayer: {project.DependencyLayerInternalReadOnly}");
        Console.WriteLine("Dependencies:");

        foreach (var reference in project.DependenciesReadOnly.Values)
        {
            Console.WriteLine($"	{reference.UsingReadOnly.NameReadOnly}");
            Console.WriteLine($"	Type: {reference.ReferenceTypeReadOnly}");
            Console.WriteLine(new string('-', 50));
        }

        if (parameters.Contains("-a"))
        {
            Console.WriteLine("indirect Dependencies:");


            foreach (var reference in project.GetAllDependenciesRecursive(parameters.Contains("-i")).Values)
            {
                Console.WriteLine($"	{reference.NameReadOnly}");
                Console.WriteLine(new string('-', 50));
            }
        }

        Console.WriteLine("References:");
        foreach (var reference in project.ReferencesReadOnly.Values)
        {
            Console.WriteLine($"	{reference.UsedByReadOnly.NameReadOnly}");
            Console.WriteLine($"	Type: {reference.ReferenceTypeReadOnly}");
            Console.WriteLine(new string('-', 50));
        }


        if (parameters.Contains("-a"))
        {
            Console.WriteLine("indirect References:");

            foreach (var reference in project.GetAllReferencesRecursive().Values)
            {
                Console.WriteLine($"	{reference.NameReadOnly}");
                Console.WriteLine(new string('-', 50));
            }
        }

        if (parameters.Contains("-ns"))
        {
            Console.WriteLine("Namespaces:");

            foreach (var projectNamespace in project.NamespacesReadOnly.Values)
            {
                Console.WriteLine($"	{projectNamespace.NameReadOnly}");

                if (parameters.Contains("-t"))
                {
                    Console.WriteLine("Types:");
                    foreach (var namespaceType in projectNamespace.NamespaceTypesReadOnly.Values)
                    {
                        Console.WriteLine($"		{namespaceType.NameReadOnly}");
                        Console.WriteLine(new string('-', 50));
                        //ToDo UD / UsingTypes for Namespace
                    }
                }

                Console.WriteLine(new string('-', 50));
            }
        }
    }

    private void ListProjects(IList<string> parameters)
    {
        var projects = _readOnlyProjectProvider.AllProjectsReadOnly.Values.ToList();

        if (parameters.Contains("-i"))
        {
            projects = _readOnlyProjectProvider.InternalProjectsReadOnly.Values.Select(x => x as IReadOnlyProject)
                .ToList();
        }

        if (parameters.Contains("-e"))
        {
            projects = _readOnlyProjectProvider.ExternalProjectsReadOnly.Values.Select(x => x as IReadOnlyProject)
                .ToList();
        }

        if (parameters.Contains("-t"))
        {
            projects = parameters.Contains("-i")
                ? _projectQueries.GetInternalTopLevelProjects().Values.Select(x => x as IReadOnlyProject).ToList()
                : _projectQueries.GetTopLevelProjects().Values.ToList();
        }

        if (parameters.Contains("-s"))
        {
            projects = _projectQueries.GetSubLevelProjects().Values.ToList();
        }

        if (parameters.Contains("-gdl") || parameters.Contains("-grl"))
        {
            var layerGrouping = new List<IGrouping<int, IReadOnlyProject>>();
            if (parameters.Contains("-gdl"))
            {
                layerGrouping = parameters.Contains("-i")
                    ? projects.GroupBy(x => x.DependencyLayerInternalReadOnly).ToList()
                    : projects.GroupBy(x => x.DependencyLayerReadOnly).ToList();
            }
            else if (parameters.Contains("-grl"))
            {
                layerGrouping = projects.GroupBy(x => x.ReferenceLayerReadOnly).ToList();
            }

            foreach (var layer in layerGrouping.OrderBy(x => x.Key))
            {
                Console.WriteLine($"Layer: {layer.Key}");
                Console.WriteLine(new string('-', 50));

                foreach (var readOnlyProject in layer)
                {
                    Console.WriteLine(readOnlyProject.NameReadOnly);
                    Console.WriteLine(readOnlyProject.ProjectTypeReadOnly);
                }
            }
        }
        else
        {
            foreach (var readOnlyProject in projects)
            {
                Console.WriteLine(new string('-', 50));
                Console.WriteLine(readOnlyProject.NameReadOnly);
                Console.WriteLine(readOnlyProject.ProjectTypeReadOnly);
            }
        }
    }

    private void Exit(IList<string> parameters)
    {
        _applicationLifetime.StopApplication();
    }

    private void ReloadCaches(IList<string> parameters)
    {
        _cacheManager.ReloadCaches();
    }

    private void ActivateCache(IList<string> parameters)
    {
        var cacheString = parameters.First();
        var cache = _cacheManager.Caches.FirstOrDefault(x =>
            x.Id.ToString().ToLower() == cacheString || x.Name?.ToLower() == cacheString);

        if (cache is null)
        {
            _logger.LogWarning($"Cache not found: {cacheString}!");
            return;
        }

        _cacheManager.ActivateCache(cache.Id);
    }

    private void DeleteCache(IList<string> parameters)
    {
        var cacheString = parameters.First();
        var cache = _cacheManager.Caches.FirstOrDefault(x =>
            x.Id.ToString().ToLower() == cacheString || x.Name?.ToLower() == cacheString);

        if (cache is null)
        {
            _logger.LogWarning($"Cache not found: {cacheString}!");
            return;
        }

        _cacheManager.DeleteCache(cache.Id);
    }

    private void ListCaches(IList<string> parameters)
    {
        var caches = _cacheManager.Caches;

        foreach (var cache in caches)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"{cache.Name}");
            Console.WriteLine($"{cache.Id}");
            Console.WriteLine($"{cache.State.ToString()}");
            Console.WriteLine($"{cache.Timestamp}");
        }
    }

    private void Load()
    {
        _cacheManager.LoadCaches();

        if (!_cacheManager.Caches.Any(x => x.State is CacheState.Active))
        {
            _projectLoader.LoadAllProjects();
        }
    }

    private void ProcessInput()
    {
        Console.WriteLine("Waiting for Input:");
        var input = Console.ReadLine();
        Console.Clear();

        if (input is null)
        {
            return;
        }

        input = input.ToLower();

        var command = _commands.FirstOrDefault(x => x.CommandStrings.Any(y => input.StartsWith(y)));

        if (command is null)
        {
            Console.WriteLine("Command not found!");
            return;
        }

        var commandString = command.CommandStrings.First(x => input.StartsWith(x));
        var parameter = input.Remove(commandString)
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parameter.Length < command.RequiredParameters)
        {
            Console.WriteLine("Not enough parameters provided.");
            return;
        }

        try
        {
            command.Action.Invoke(parameter);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }

        Console.WriteLine("");
        Console.WriteLine(new string('=', 50));
    }

    private void SaveCache(IList<string> parameters)
    {
        if (!parameters.Any())
        {
            _cacheManager.SaveAsCurrentCache();
            return;
        }

        var existingCache = _cacheManager.Caches.FirstOrDefault(x => x.Name == parameters.First());
        if (existingCache is not null)
        {
            _cacheManager.SaveAsExistingCache(existingCache.Id);
            return;
        }

        _cacheManager.SaveAsNewCache(parameters.First());
    }
}