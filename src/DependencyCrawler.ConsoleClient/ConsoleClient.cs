using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;

namespace DependencyCrawler.ConsoleClient;

public class ConsoleClient : IConsoleClient
{
	private readonly ICacheManager _cacheManager;
	private readonly IEnumerable<Command> _commands;
	private readonly IProjectLoader _projectLoader;
	private readonly IProjectQueriesReadOnly _projectQueries;
	private readonly IReadOnlyProjectProvider _readOnlyProjectProvider;

	public ConsoleClient(IReadOnlyProjectProvider readOnlyProjectProvider, IProjectLoader projectLoader,
		IProjectQueriesReadOnly projectQueries, ICacheManager cacheManager)
	{
		_readOnlyProjectProvider = readOnlyProjectProvider;
		_projectLoader = projectLoader;
		_projectQueries = projectQueries;
		_cacheManager = cacheManager;

		_commands = new List<Command>
		{
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

	private void ListCaches(IList<string> parameters)
	{
		var caches = _cacheManager.Caches;

		foreach (var cache in caches)
		{
			Console.WriteLine(new string('-', 50));
			Console.WriteLine($"{cache.Name}");
			Console.WriteLine($"{cache.Id}");
			Console.WriteLine($"{cache.Timestamp}");
		}
	}

	private void Load()
	{
		_cacheManager.LoadCaches();
		var caches = _cacheManager.Caches;

		if (!caches.Any())
		{
			_projectLoader.LoadAllProjects();
			return;
		}

		Console.WriteLine("Caches detected:");
		foreach (var cache in caches)
		{
			Console.WriteLine(new string('-', 50));
			Console.WriteLine($"{cache.Name}");
			Console.WriteLine($"{cache.Id}");
			Console.WriteLine($"{cache.Timestamp}");
		}

		Console.WriteLine("");
		Console.WriteLine("Select cache to load or type load to load the current project-files.");


		var input = Console.ReadLine();

		while (input is null || input is "load" || !caches.Any(x => x.Id.ToString() == input || x.Name == input))
		{
			input = Console.ReadLine();
		}


		if (input is "load")
		{
			_projectLoader.LoadAllProjects();
			//ToDo Output
			return;
		}

		var selectedCache = caches.First(x => x.Id.ToString() == input || x.Name == input);

		_cacheManager.ActivateCache(selectedCache.Id);
	}

	private void ProcessInput()
	{
		var input = Console.ReadLine();

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
		var parameter = input.Remove(commandString).Split(" ");

		if (parameter.Length < command.RequiredParameters)
		{
			Console.WriteLine("Not enough parameters provided.");
			return;
		}

		command.Action.Invoke(parameter);
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

public class Command
{
	public required int RequiredParameters { get; init; }
	public required IList<string> CommandStrings { get; init; }
	public required Action<IList<string>> Action { get; init; }
}