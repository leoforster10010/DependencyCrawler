using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models;
using DependencyCrawler.Implementations.Models.CachedTypes;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.Loader;

internal class ProjectLoader : IProjectLoader
{
	private readonly IDllFileProvider _dllFileProvider;
	private readonly ILinkedTypeFactory _linkedTypeFactory;
	private readonly ILogger<ProjectLoader> _logger;
	private readonly IProjectFileProvider _projectFileProvider;
	private readonly IProjectInfoFactory _projectInfoFactory;
	private readonly IProjectProvider _projectProvider;
	private Cache? _cache;

	public ProjectLoader(IProjectProvider projectProvider, IProjectFileProvider projectFileProvider,
		IProjectInfoFactory projectInfoFactory, IDllFileProvider dllFileProvider, ILinkedTypeFactory linkedTypeFactory,
		ILogger<ProjectLoader> logger)
	{
		_projectProvider = projectProvider;
		_projectFileProvider = projectFileProvider;
		_projectInfoFactory = projectInfoFactory;
		_dllFileProvider = dllFileProvider;
		_linkedTypeFactory = linkedTypeFactory;
		_logger = logger;
	}

	public void LoadAllProjects()
	{
		_projectProvider.Clear();
		var csprojFiles = _projectFileProvider.GetProjectFiles();

		foreach (var csprojFile in csprojFiles)
		{
			LoadInternalProject(csprojFile);
		}

		RemoveReferenceLoops();
		LinkUsingDirectives();
	}

	public void LoadProjectsFromCache(Cache cache)
	{
		_projectProvider.Clear();
		_cache = cache;

		foreach (var cachedProject in cache.CachedProjects)
		{
			LoadProjectFromCache(cachedProject);
		}

		RemoveReferenceLoops();
		LinkUsingDirectives();
	}

	private void LoadProjectFromCache(Guid id)
	{
		var cachedProject = _cache?.CachedProjects.FirstOrDefault(x => x.Id == id);

		if (cachedProject is null)
		{
			_logger.LogError("Project not found in cache or cache is null!");
			return;
		}

		LoadProjectFromCache(cachedProject);
	}

	private void LoadProjectFromCache(CachedProject cachedProject)
	{
		switch (cachedProject.ProjectType)
		{
			case ProjectType.Internal:
				LoadInternalProject(cachedProject);
				break;
			case ProjectType.External:
				LoadExternalProject(cachedProject);
				break;
			case ProjectType.Unresolved:
				LoadUnresolvedProject(cachedProject);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void LoadProject(string name)
	{
		//ToDo Rework: consider version, if PackageReference load as external

		_logger.LogInformation($"Loading {name}...");
		if (_projectFileProvider.GetIsInternal(name))
		{
			var csprojFile = _projectFileProvider.GetProjectFile(name);
			if (csprojFile is not null)
			{
				LoadInternalProject(csprojFile);
			}

			return;
		}

		if (_dllFileProvider.GetIsExternal(name))
		{
			var dllFile = _dllFileProvider.GetDllFile(name);
			if (dllFile is not null)
			{
				LoadExternalProject(dllFile);
			}

			return;
		}

		LoadUnresolvedProject(name);
	}

	private void RemoveReferenceLoops()
	{
		_logger.LogInformation("Removing reference-loops...");

		var sublevelProjects = _projectProvider.AllProjects.Where(x => !x.Value.References.Any());
		var exploredProjects = new Dictionary<Guid, IProject>();

		foreach (var project in sublevelProjects)
		{
			project.Value.RemoveReferenceLoops(exploredProjects: exploredProjects);
		}

		_logger.LogInformation("Reference-loops removed.");
	}

	private void LinkUsingDirectives()
	{
		_logger.LogInformation("Linking UsingDirectives...");

		var internalProjects = _projectProvider.InternalProjects.ToList();
		var unlinkedUsingDirectives = internalProjects.SelectMany(x =>
				x.Value.Namespaces.Values.SelectMany(y =>
					y.NamespaceTypes.Values.SelectMany(z => z.UsingDirectives.Values)))
			.Where(x => x.State == TypeUsingDirectiveState.Unlinked).ToList();

		var namespaces = _projectProvider.AllProjects.SelectMany(x => x.Value.Namespaces.Values).ToList();

		var namespacesDictionary = _projectProvider.AllProjects.SelectMany(x => x.Value.Namespaces.Values)
			.GroupBy(x => x.Name)
			.ToDictionary(x => x.Key, x => x.First());

		var types = namespaces.SelectMany(x => x.NamespaceTypes.Values)
			.GroupBy(x => x.Name)
			.ToDictionary(x => x.Key, x => x.First());

		var typesFullname = namespaces.SelectMany(x => x.NamespaceTypes.Values)
			.GroupBy(x => x.FullName)
			.ToDictionary(x => x.Key, x => x.First());

		_logger.LogInformation($"{unlinkedUsingDirectives.Count} unlinked UsingDirectives found");

		foreach (var usingDirective in unlinkedUsingDirectives)
		{
			if (namespacesDictionary.TryGetValue(usingDirective.Name, out var referencedNamespace))
			{
				usingDirective.ReferencedNamespace = referencedNamespace;
				usingDirective.State = TypeUsingDirectiveState.Linked;
				referencedNamespace.UsingTypes.TryAdd(usingDirective.Id, usingDirective.ParentType);

				continue;
			}

			if (types.TryGetValue(usingDirective.Name, out var referencedType))
			{
				usingDirective.ReferencedNamespace = referencedType.ParentNamespace;
				usingDirective.State = TypeUsingDirectiveState.Linked;
				referencedType.ParentNamespace.UsingTypes.TryAdd(usingDirective.Id, usingDirective.ParentType);

				continue;
			}

			if (typesFullname.TryGetValue(usingDirective.Name, out referencedType))
			{
				usingDirective.ReferencedNamespace = referencedType.ParentNamespace;
				usingDirective.State = TypeUsingDirectiveState.Linked;
				referencedType.ParentNamespace.UsingTypes.TryAdd(usingDirective.Id, usingDirective.ParentType);

				continue;
			}

			usingDirective.State = TypeUsingDirectiveState.Unresolved;
		}

		var linkedDirectivesCount = unlinkedUsingDirectives.Count(x => x.State == TypeUsingDirectiveState.Linked);
		var unresolvedDirectivesCount =
			unlinkedUsingDirectives.Count(x => x.State == TypeUsingDirectiveState.Unresolved);

		_logger.LogInformation($"{linkedDirectivesCount} UsingDirectives linked.");
		_logger.LogWarning($"{unresolvedDirectivesCount} UsingDirectives remain unresolved.");
	}

	private IProject GetProjectOrLoad(string name)
	{
		if (_projectProvider.AllProjects.TryGetValue(name, out var project))
		{
			return project;
		}

		LoadProject(name);
		var newProject = _projectProvider.AllProjects[name];

		return newProject;
	}

	private IProject GetProjectOrLoadFromCache(string name, Guid id)
	{
		if (_projectProvider.AllProjects.TryGetValue(name, out var project))
		{
			return project;
		}

		LoadProjectFromCache(id);
		var newProject = _projectProvider.AllProjects[name];

		return newProject;
	}

	private void LoadUnresolvedProject(string name)
	{
		var unresolvedProject = _linkedTypeFactory.CreateUnresolvedProject(name);
		_projectProvider.AddUnresolvedProject(unresolvedProject);
	}

	private void LoadInternalProject(string csprojFile)
	{
		if (_projectProvider.InternalProjects.Any(x => x.Value.Name == csprojFile.GetProjectName()))
		{
			return;
		}

		var internalProjectInfo = _projectInfoFactory.GetInternalProjectInfo(csprojFile);

		var internalProject = _linkedTypeFactory.CreateInternalProject(internalProjectInfo);

		_projectProvider.AddInternalProject(internalProject);

		foreach (var packageReferenceInfo in internalProjectInfo.PackageReferences)
		{
			var referencedProject = GetProjectOrLoad(packageReferenceInfo.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(packageReferenceInfo, internalProject, referencedProject);
			internalProject.PackageReferences.TryAdd(packageReference.Id, packageReference);
		}

		foreach (var projectReferenceInfo in internalProjectInfo.ProjectReferences)
		{
			var referencedProject = GetProjectOrLoad(projectReferenceInfo.Using);
			var projectReference =
				_linkedTypeFactory.GetProjectReference(internalProject, referencedProject);
			internalProject.ProjectReferences.TryAdd(projectReference.Id, projectReference);
		}
	}

	private void LoadExternalProject(string dllFile)
	{
		if (_projectProvider.ExternalProjects.Any(x => x.Value.Name == dllFile.GetDllName()))
		{
			return;
		}

		var externalProjectInfo = _projectInfoFactory.GetExternalProjectInfo(dllFile);

		var externalProject = _linkedTypeFactory.CreateExternalProject(externalProjectInfo);

		_projectProvider.AddExternalProject(externalProject);

		foreach (var packageReferenceInfo in externalProjectInfo.PackageReferences)
		{
			var referencedProject = GetProjectOrLoad(packageReferenceInfo.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(packageReferenceInfo, externalProject, referencedProject);
			externalProject.PackageReferences.TryAdd(packageReference.Id, packageReference);
		}
	}

	private void LoadInternalProject(CachedProject cachedProject)
	{
		if (_projectProvider.AllProjects.ContainsKey(cachedProject.Name))
		{
			return;
		}

		var internalProject = _linkedTypeFactory.CreateInternalProject(cachedProject);

		_projectProvider.AddInternalProject(internalProject);

		foreach (var cachedPackageReference in cachedProject.PackageReferences)
		{
			var referencedProject =
				GetProjectOrLoadFromCache(cachedPackageReference.UsedProjectName, cachedPackageReference.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(cachedPackageReference, internalProject, referencedProject);
			internalProject.PackageReferences.TryAdd(packageReference.Id, packageReference);
		}

		foreach (var cachedProjectReference in cachedProject.ProjectReferences)
		{
			var referencedProject =
				GetProjectOrLoadFromCache(cachedProjectReference.UsedProjectName, cachedProjectReference.Using);
			var projectReference =
				_linkedTypeFactory.GetProjectReference(cachedProjectReference, internalProject, referencedProject);
			internalProject.ProjectReferences.TryAdd(projectReference.Id, projectReference);
		}
	}

	private void LoadExternalProject(CachedProject cachedProject)
	{
		if (_projectProvider.AllProjects.ContainsKey(cachedProject.Name))
		{
			return;
		}

		var externalProject = _linkedTypeFactory.CreateExternalProject(cachedProject);

		_projectProvider.AddExternalProject(externalProject);

		foreach (var cachedPackageReference in cachedProject.PackageReferences)
		{
			var referencedProject =
				GetProjectOrLoadFromCache(cachedPackageReference.UsedProjectName, cachedPackageReference.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(cachedPackageReference, externalProject, referencedProject);
			externalProject.PackageReferences.TryAdd(packageReference.Id, packageReference);
		}
	}

	private void LoadUnresolvedProject(CachedProject cachedProject)
	{
		var unresolvedProject = _linkedTypeFactory.CreateUnresolvedProject(cachedProject);
		_projectProvider.AddUnresolvedProject(unresolvedProject);
	}
}