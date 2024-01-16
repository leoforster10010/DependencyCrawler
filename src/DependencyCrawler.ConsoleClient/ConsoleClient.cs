using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Framework.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.ConsoleClient;

public class ConsoleClient : IConsoleClient
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ICacheManager _cacheManager;
    private readonly IEnumerable<Command> _commands;
    private readonly IConfigurationValidator _configurationValidator;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<ConsoleClient> _logger;
    private readonly IProjectLoader _projectLoader;
    private readonly IProjectQueriesReadOnly _projectQueries;
    private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

    public ConsoleClient(IReadOnlyProjectProvider readOnlyProjectProvider, IProjectLoader projectLoader,
        IProjectQueriesReadOnly projectQueries, ICacheManager cacheManager, ILogger<ConsoleClient> logger,
        IHostApplicationLifetime applicationLifetime, IEvaluationRepository evaluationRepository,
        IConfigurationValidator configurationValidator)
    {
        _readOnlyProjectProvider = readOnlyProjectProvider;
        _projectLoader = projectLoader;
        _projectQueries = projectQueries;
        _cacheManager = cacheManager;
        _logger = logger;
        _applicationLifetime = applicationLifetime;
        _evaluationRepository = evaluationRepository;
        _configurationValidator = configurationValidator;

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
                Action = Exit,
                Description = "Closes the application."
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "help",
                    "h"
                },
                Action = Help,
                Description = "Shows the available commands."
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "evaluate",
                    "eval"
                },
                Action = Evaluate,
                Description = "Evaluates the current dependency-structure."
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
                Action = LoadFiles,
                Description =
                    "Loads the dependency-structure of the csproj-files contained in the folder defined by the appsettings.json."
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "unresolvedUsingDirectives",
                    "uud"
                },
                Action = ListUnresolvedUsingDirectives,
                Description = "Lists the unresolved using-directives."
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
                Action = SaveCache,
                Description = "Saves the loaded data as the currently active cache. " + Environment.NewLine +
                              "If a name is specified as parameter, a cache with this name is created. " +
                              Environment.NewLine +
                              "If no cache with this name exists, a new one is created."
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
                Action = ListCaches,
                Description = "Lists the available caches."
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
                Action = DeleteCache,
                Description = "Deletes a cache. Required parameter: name or id of the cache."
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
                Action = ActivateCache,
                Description = "Activates a cache. Required parameter: name or id of the cache."
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
                Action = ReloadCaches,
                Description = "Reloads all caches."
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
                Action = ListProjects,
                Description = "Lists all projects." + Environment.NewLine +
                              "Available Flags:" + Environment.NewLine +
                              "-i: lists only internal projects" + Environment.NewLine +
                              "-e: lists only external projects" + Environment.NewLine +
                              "-t: lists the top-level projects" + Environment.NewLine +
                              "-s: lists the sub-level projects" + Environment.NewLine +
                              "-gdl: groups the projects by dependency-layer" + Environment.NewLine +
                              "-grl: groups the projects by reference-layer"
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
                Action = ShowProjectInfo,
                Description = "Shows the details of a project. Required parameter: name of project." +
                              Environment.NewLine +
                              "Available Flags:" + Environment.NewLine +
                              "-a: Lists the indirect dependencies and references" + Environment.NewLine +
                              "-r: Lists the dependencies, required by using-Directives. If used i combination with -a, unresolved and required projects also get listed." +
                              Environment.NewLine +
                              "-ns: Lists the namespaces of the project" + Environment.NewLine +
                              "-t: Lists the types of the project"
            },
            new()
            {
                RequiredParameters = 2,
                CommandStrings = new List<string>
                {
                    "dependency path",
                    "dependency p",
                    "d path",
                    "d p"
                },
                Action = ShowDependencyPath,
                Description = "Shows the dependency paths between two projects." + Environment.NewLine +
                              "Required parameters: two project names"
            },
            new()
            {
                RequiredParameters = 0,
                CommandStrings = new List<string>
                {
                    "average distance",
                    "average d",
                    "a distance",
                    "a d"
                },
                Action = AveragePathDistance,
                Description =
                    "Shows the average dependency path distance to all referencing projects for a specific or all projects." +
                    Environment.NewLine +
                    "Optional parameters: name of project"
            }
        };
    }


    public void Run(CancellationToken cancellationToken)
    {
        //Load from cache or files
        Load();

        //Idle for input
        while (cancellationToken is not { IsCancellationRequested: true })
        {
            ProcessInput();
        }
    }

    private void AveragePathDistance(IList<string> parameters)
    {
        if (parameters.Any())
        {
            var project =
                _readOnlyProjectProvider.AllProjectsReadOnly.Values.FirstOrDefault(x =>
                    x.NameReadOnly.ToLower() == parameters.First());

            if (project is null)
            {
                _logger.LogWarning($"Project not found: {parameters.First()}");
                return;
            }

            Console.WriteLine($"{project.NameReadOnly}: {project.GetAverageDependencyPathDistance()}");
            return;
        }

        var internalProjects = _readOnlyProjectProvider.InternalProjectsReadOnly.Values;
        foreach (var internalProject in internalProjects)
        {
            Console.WriteLine($"{internalProject.NameReadOnly}: {internalProject.GetAverageDependencyPathDistance()}");
        }
    }

    private void ShowDependencyPath(IList<string> parameters)
    {
        var project =
            _readOnlyProjectProvider.AllProjectsReadOnly.Values.FirstOrDefault(x =>
                x.NameReadOnly.ToLower() == parameters.First());
        var dependency =
            _readOnlyProjectProvider.AllProjectsReadOnly.Values.FirstOrDefault(x =>
                x.NameReadOnly.ToLower() == parameters[1]);
        if (project is null)
        {
            _logger.LogWarning($"Project not found: {parameters.First()}");
            return;
        }

        if (dependency is null)
        {
            _logger.LogWarning($"Project not found: {parameters[1]}");
            return;
        }

        var dependencyPathInfo = project.GetDependencyPathInfo(dependency);

        foreach (var dependencyPath in dependencyPathInfo.PossiblePaths)
        {
            var projectNames = dependencyPath.Select(x => x.NameReadOnly);
            Console.WriteLine(string.Join(" -> ", projectNames));
            Console.WriteLine("");
        }

        Console.WriteLine($"Count possible paths: {dependencyPathInfo.PossiblePaths.Count}");
        Console.WriteLine($"Distance shortest path: {dependencyPathInfo.DistanceShortestPath}");
        Console.WriteLine($"Distance longest path: {dependencyPathInfo.DistanceLongestPath}");
    }

    private void Help(IList<string> parameters)
    {
        foreach (var command in _commands)
        {
            Console.WriteLine(command.Action.Method.Name);
            Console.WriteLine(command.Description);
            Console.WriteLine("Commands:");
            foreach (var commandString in command.CommandStrings)
            {
                Console.WriteLine($"{commandString}");
            }

            Console.WriteLine("");
        }
    }

    private void Evaluate(IList<string> parameters)
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
            .Where(x => x is { StateReadOnly: TypeUsingDirectiveState.Unresolved })
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


            foreach (var reference in project.GetAllDependenciesReadOnlyRecursive(parameters.Contains("-i")).Values)
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

            foreach (var reference in project.GetAllReferencesReadOnlyRecursive().Values)
            {
                Console.WriteLine($"	{reference.NameReadOnly}");
                Console.WriteLine(new string('-', 50));
            }
        }

        if (parameters.Contains("-r"))
        {
            Console.WriteLine("required Dependencies:");

            foreach (var reference in project.GetRequiredDependenciesReadOnly(parameters.Contains("-a")).Values)
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
        ValidateConfiguration();

        _cacheManager.LoadCaches();

        if (!_cacheManager.Caches.Any(x => x.State is CacheState.Active))
        {
            _projectLoader.LoadAllProjects();
        }
    }

    private void ValidateConfiguration()
    {
        while (!_configurationValidator.IsConfigurationValid())
        {
            Console.WriteLine(
                "The following configurations are invalid. Please update the appsettings.json accordingly.");

            foreach (var invalidConfiguration in _configurationValidator.GetInvalidConfigurations())
            {
                Console.WriteLine($"- {invalidConfiguration}");
            }

            Console.WriteLine("");
            Console.WriteLine("Press enter to continue.");
            Console.ReadKey();
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