using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.Loader;

public class ProjectLoader : IProjectLoader
{
	private readonly IDllFileProvider _dllFileProvider;
	private readonly ILinkedTypeFactory _linkedTypeFactory;
	private readonly ILogger<ProjectLoader> _logger;
	private readonly IProjectFileProvider _projectFileProvider;
	private readonly IProjectInfoFactory _projectInfoFactory;
	private readonly IProjectProvider _projectProvider;

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
		var csprojFiles = _projectFileProvider.GetProjectFiles();

		foreach (var csprojFile in csprojFiles)
		{
			LoadInternalProject(csprojFile);
		}

		LinkUsingDirectives();
	}

	public IProject LoadProjectByName(string name)
	{
		LoadProject(name);
		LinkUsingDirectives();
		return _projectProvider.AllProjects[name];
	}

	private void LoadProject(string name)
	{
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

	private void LinkUsingDirectives()
	{
		_logger.LogInformation("Linking UsingDirectives...");

		var internalProjects = _projectProvider.InternalProjects.ToList();
		var unlinkedUsingDirectives = internalProjects.SelectMany(x =>
				x.Namespaces.Values.SelectMany(y =>
					y.NamespaceTypes.Values.SelectMany(z => z.UsingDirectives.Values)))
			.Where(x => x.State == TypeUsingDirectiveState.Unlinked).ToList();

		_logger.LogInformation($"{unlinkedUsingDirectives.Count} unlinked UsingDirectives found");

		foreach (var usingDirective in unlinkedUsingDirectives)
		{
			var referencedProject =
				_projectProvider.AllProjects.Values.FirstOrDefault(x => x.Namespaces.ContainsKey(usingDirective.Name));

			if (referencedProject is null)
			{
				usingDirective.State = TypeUsingDirectiveState.Unresolved;
				continue;
			}

			var referencedNamespace = referencedProject.Namespaces[usingDirective.Name];

			usingDirective.ReferencedNamespace = referencedNamespace;
			usingDirective.State = TypeUsingDirectiveState.Linked;
			referencedNamespace.UsingTypes.TryAdd(usingDirective.Name, usingDirective.ParentType);
		}

		var linkedDirectivesCount = unlinkedUsingDirectives.Count(x => x.State == TypeUsingDirectiveState.Linked);
		var unresolvedDirectivesCount =
			unlinkedUsingDirectives.Count(x => x.State == TypeUsingDirectiveState.Unresolved);

		_logger.LogInformation($"{linkedDirectivesCount} UsingDirectives linked.");
		_logger.LogWarning($"{unresolvedDirectivesCount} UsingDirectives remain unresolved.");
	}

	private IProject GetProject(string name)
	{
		if (_projectProvider.AllProjects.TryGetValue(name, out var project))
		{
			return project;
		}

		LoadProject(name);
		var newProject = _projectProvider.AllProjects[name];

		return newProject;
	}

	private void LoadUnresolvedProject(string name)
	{
		_projectProvider.AddUnresolvedProject(name);
	}

	private void LoadInternalProject(string csprojFile)
	{
		if (_projectProvider.InternalProjects.Any(x => x.Name == csprojFile.GetProjectName()))
		{
			return;
		}

		var internalProjectInfo = _projectInfoFactory.GetInternalProjectInfo(csprojFile);

		var internalProject = new InternalProject
		{
			ProjectRootElement = internalProjectInfo.ProjectRootElement,
			Name = internalProjectInfo.Name
		};
		_projectProvider.AddInternalProject(internalProject);

		foreach (var packageReferenceInfo in internalProjectInfo.PackageReferences)
		{
			var referencedProject = GetProject(packageReferenceInfo.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(packageReferenceInfo, internalProject, referencedProject);
			internalProject.PackageReferences.TryAdd(packageReference.Using.Name, packageReference);
		}

		foreach (var projectReferenceInfo in internalProjectInfo.ProjectReferences)
		{
			var referencedProject = GetProject(projectReferenceInfo.Using);
			var projectReference =
				_linkedTypeFactory.GetProjectReference(projectReferenceInfo, internalProject, referencedProject);
			internalProject.ProjectReferences.TryAdd(projectReference.Using.Name, projectReference);
		}

		foreach (var namespaceInfo in internalProjectInfo.Namespaces)
		{
			var projectNamespace = _linkedTypeFactory.GetProjectNamespace(namespaceInfo, internalProject);
			internalProject.Namespaces.TryAdd(projectNamespace.Name, projectNamespace);
		}
	}

	private void LoadExternalProject(string dllFile)
	{
		if (_projectProvider.ExternalProjects.Any(x => x.Name == dllFile.GetDllName()))
		{
			return;
		}

		var externalProjectInfo = _projectInfoFactory.GetExternalProjectInfo(dllFile);

		var externalProject = new ExternalProject
		{
			Assembly = externalProjectInfo.Assembly,
			Name = externalProjectInfo.Name
		};
		_projectProvider.AddExternalProject(externalProject);

		foreach (var packageReferenceInfo in externalProjectInfo.PackageReferences)
		{
			var referencedProject = GetProject(packageReferenceInfo.Using);
			var packageReference =
				_linkedTypeFactory.GetPackageReference(packageReferenceInfo, externalProject, referencedProject);
			externalProject.PackageReferences.TryAdd(packageReference.Using.Name, packageReference);
		}

		foreach (var namespaceInfo in externalProjectInfo.Namespaces)
		{
			var projectNamespace = _linkedTypeFactory.GetProjectNamespace(namespaceInfo, externalProject);
			externalProject.Namespaces.TryAdd(projectNamespace.Name, projectNamespace);
		}
	}
}