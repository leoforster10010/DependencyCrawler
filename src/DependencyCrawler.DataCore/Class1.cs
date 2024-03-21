using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore;

internal enum ProjectType
{
	CSharp
}

internal interface IDependencyCrawlerCore
{
	ConcurrentDictionary<string, Project> Projects { get; }
	Guid Id { get; }
	IDependencyCrawlerCoreProvider DependencyCrawlerCoreProvider { get; init; }
	bool IsActive { get; }
}

internal interface IDependencyCrawlerReadonlyCore
{
	IReadOnlyDictionary<string, IReadOnlyProject> ProjectsReadOnly { get; }
	Guid Id { get; }
}

internal interface IDependencyCrawlerValueCore
{
	IReadOnlyDictionary<string, IValueProject> ProjectsValue { get; }
	Guid Id { get; }
}

internal class DependencyCrawlerCore : IDependencyCrawlerCore, IDependencyCrawlerReadonlyCore, IDependencyCrawlerValueCore
{
	public bool IsActive => DependencyCrawlerCoreProvider.ActiveCore.Id == Id;
	public Guid Id { get; } = Guid.NewGuid();
	public required IDependencyCrawlerCoreProvider DependencyCrawlerCoreProvider { get; init; }
	public ConcurrentDictionary<string, Project> Projects { get; } = new();
	public IReadOnlyDictionary<string, IReadOnlyProject> ProjectsReadOnly => Projects.ToDictionary(key => key.Key, value => value.Value as IReadOnlyProject);
	public IReadOnlyDictionary<string, IValueProject> ProjectsValue => Projects.ToDictionary(key => key.Key, value => value.Value as IValueProject);
}

internal interface IProject : IReadOnlyProject, IValueProject
{
	Guid Id { get; }
	ProjectType Type { get; init; }
	string Name { get; set; }
	ConcurrentDictionary<string, Module> Modules { get; }
}

internal class Project : IProject
{
	public Guid Id { get; } = Guid.NewGuid();
	public required ProjectType Type { get; init; }
	public string Name { get; set; } = string.Empty;
	public ConcurrentDictionary<string, Module> Modules { get; } = new();


	public ProjectType TypeReadOnly => Type;
	public string NameReadOnly => Name;
	public IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly => Modules.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);


	public ProjectType TypeValue => Type;
	public string NameValue => Name;
	public IReadOnlyDictionary<string, IValueModule> ModulesValue => Modules.ToDictionary(key => key.Key, value => value.Value as IValueModule);
}

internal interface IValueProject
{
	ProjectType TypeValue { get; }
	string NameValue { get; }
	IReadOnlyDictionary<string, IValueModule> ModulesValue { get; }
}

internal interface IReadOnlyProject
{
	ProjectType TypeReadOnly { get; }
	string NameReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> ModulesReadOnly { get; }
}

internal interface IModule : IReadOnlyModule, IValueModule
{
	string Name { get; init; }
	Project Project { get; init; }
	ConcurrentDictionary<string, Module> DependingOn { get; }
	ConcurrentDictionary<string, Module> DependencyOf { get; }
}

internal class Module : IModule
{
	public required string Name { get; init; }
	public required Project Project { get; init; }

	public ConcurrentDictionary<string, Module> DependingOn { get; } = new();
	public ConcurrentDictionary<string, Module> DependencyOf { get; } = new();


	public string NameReadOnly => Name;
	public IReadOnlyProject ProjectReadOnly => Project;

	public IReadOnlyDictionary<string, IReadOnlyModule> DependingOnReadOnly => DependingOn.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IReadOnlyModule> DependencyOfReadOnly => DependencyOf.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);


	public string NameValue => Name;
	public Guid ProjectValue => Project.Id;

	public IReadOnlyDictionary<string, IReadOnlyModule> DependingOnValue => DependingOn.ToDictionary(key => key.Key, value => value.Value as IReadOnlyModule);
	public IReadOnlyDictionary<string, IValueModule> DependencyOfValue => DependencyOf.ToDictionary(key => key.Key, value => value.Value as IValueModule);
}

internal interface IReadOnlyModule
{
	string NameReadOnly { get; }
	IReadOnlyProject ProjectReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependingOnReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependencyOfReadOnly { get; }
}

internal interface IValueModule
{
	string NameValue { get; }
	Guid ProjectValue { get; }
	IReadOnlyDictionary<string, IReadOnlyModule> DependingOnValue { get; }
	IReadOnlyDictionary<string, IValueModule> DependencyOfValue { get; }
}

internal interface IDependencyCrawlerCoreProvider
{
	DependencyCrawlerCore ActiveCore { get; set; }
	IReadOnlyDictionary<Guid, IDependencyCrawlerValueCore> DependencyCrawlerCoresValue { get; }
	void AddCore(IDependencyCrawlerValueCore valueCore);
	void RemoveCore(Guid id);
}

internal class DependencyCrawlerCoreProvider : IDependencyCrawlerCoreProvider
{
	private readonly ConcurrentDictionary<Guid, DependencyCrawlerCore> _dependencyCrawlerCores = new();
	private DependencyCrawlerCore? _activeCore;

	public DependencyCrawlerCore ActiveCore
	{
		get
		{
			if (_activeCore is not null)
			{
				return _activeCore;
			}

			_activeCore = new DependencyCrawlerCore
			{
				DependencyCrawlerCoreProvider = this
			};
			_dependencyCrawlerCores.TryAdd(_activeCore.Id, _activeCore);
			//ToDo what if try add fails?
			return _activeCore;
		}
		set
		{
			if (!_dependencyCrawlerCores.ContainsKey(value.Id))
			{
				return;
			}

			_activeCore = value;
		}
	}

	public IReadOnlyDictionary<Guid, IDependencyCrawlerValueCore> DependencyCrawlerCoresValue => _dependencyCrawlerCores.ToDictionary(key => key.Key, value => value.Value as IDependencyCrawlerValueCore);

	public void AddCore(IDependencyCrawlerValueCore valueCore)
	{
		//ToDo VC to linked Core pipeline
	}

	public void RemoveCore(Guid id)
	{
		if (!_dependencyCrawlerCores.ContainsKey(id))
		{
			return;
		}

		var core = _dependencyCrawlerCores[id];

		if (core.IsActive)
		{
			return;
		}

		_dependencyCrawlerCores.TryRemove(core.Id, out _);
	}
}