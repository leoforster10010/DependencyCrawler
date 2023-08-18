using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using Microsoft.Extensions.Logging;

namespace DependencyCrawler.Implementations.Repositories.Factories;

internal class CachedTypeFactory : ICachedTypeFactory
{
	private readonly ICachedProjectProvider _cachedProjectProvider;
	private readonly ILogger<CachedTypeFactory> _logger;

	public CachedTypeFactory(ICachedProjectProvider cachedProjectProvider, ILogger<CachedTypeFactory> logger)
	{
		_cachedProjectProvider = cachedProjectProvider;
		_logger = logger;
	}

	public CachedProject GetCachedProject(IReadOnlyProject project)
	{
		_logger.LogInformation($"Caching {project.NameReadOnly}...");
		var cachedProject = new CachedProject
		{
			Name = project.NameReadOnly,
			ProjectType = project.ProjectTypeReadOnly
		};

		_cachedProjectProvider.AddCachedProject(cachedProject);

		cachedProject.PackageReferences = GetCachedPackageReferences(cachedProject, project);
		cachedProject.ProjectReferences = GetCachedProjectReferences(cachedProject, project);
		cachedProject.Namespaces = GetCachedNamespaces(project);

		return cachedProject;
	}

	private IList<CachedProjectNamespace> GetCachedNamespaces(IReadOnlyProject project)
	{
		var cachedProjectNamespaces = new List<CachedProjectNamespace>();

		foreach (var projectNamespace in project.NamespacesReadOnly.Values)
		{
			cachedProjectNamespaces.Add(new CachedProjectNamespace
			{
				Name = projectNamespace.NameReadOnly,
				NamespaceTypes = GetCachedTypes(projectNamespace)
			});
		}

		return cachedProjectNamespaces;
	}

	private IList<CachedNamespaceType> GetCachedTypes(IReadOnlyProjectNamespace projectNamespace)
	{
		var cachedNamespaceTypes = new List<CachedNamespaceType>();

		foreach (var namespaceType in projectNamespace.NamespaceTypesReadOnly.Values)
		{
			cachedNamespaceTypes.Add(new CachedNamespaceType
			{
				Name = namespaceType.NameReadOnly,
				UsingDirectives = GetCachedUsingDirectives(namespaceType)
			});
		}

		return cachedNamespaceTypes;
	}

	private IList<CachedTypeUsingDirective> GetCachedUsingDirectives(IReadOnlyNamespaceType namespaceType)
	{
		var cachedTypeUsingDirectives = new List<CachedTypeUsingDirective>();

		var linkedUsingDirectives =
			namespaceType.UsingDirectivesReadOnly.Values.Where(x => x.StateReadOnly is TypeUsingDirectiveState.Linked);
		foreach (var typeUsingDirective in linkedUsingDirectives)
		{
			cachedTypeUsingDirectives.Add(new CachedTypeUsingDirective
			{
				Name = typeUsingDirective.NameReadOnly,
				State = typeUsingDirective.StateReadOnly,
				ReferencedNamespaceId = GetCachedNamespaceId(typeUsingDirective.ReferencedNamespaceReadOnly)
			});
		}

		return cachedTypeUsingDirectives;
	}

	private Guid GetCachedNamespaceId(IReadOnlyProjectNamespace referencedNamespace)
	{
		var cachedNamespaceId = _cachedProjectProvider.GetCachedNamespaceId(referencedNamespace.NameReadOnly);

		if (cachedNamespaceId is not null)
		{
			return (Guid)cachedNamespaceId;
		}

		var referencedCachedProject = GetCachedProject(referencedNamespace.ParentProjectReadOnly);

		return referencedCachedProject.Namespaces.First(x => x.Name == referencedNamespace.NameReadOnly).Id;
	}


	private IList<CachedProjectReference> GetCachedProjectReferences(CachedProject cachedProject,
		IReadOnlyProject project)
	{
		//ToDo test: do we smell an exception?
		var projectReferences = project.DependenciesReadOnly.Values
			.Where(x => x.ReferenceTypeReadOnly == ReferenceType.Project)
			.Select(x => x as ProjectReference);

		var cachedProjectReferences = new List<CachedProjectReference>();

		foreach (var projectReference in projectReferences)
		{
			if (projectReference is null)
			{
				continue;
			}

			cachedProjectReferences.Add(new CachedProjectReference
			{
				Using = GetReferencedProjectId(projectReference),
				UsedProjectName = projectReference.Using.Name,
				UsedBy = cachedProject.Id
			});
		}

		return cachedProjectReferences;
	}

	private IList<CachedPackageReference> GetCachedPackageReferences(CachedProject cachedProject,
		IReadOnlyProject project)
	{
		if (project.NameReadOnly.ToLower() is "mscorlib" or "system" ||
		    project.NameReadOnly.ToLower().StartsWith("system."))
		{
			return new List<CachedPackageReference>();
		}

		//ToDo test: do we smell an exception?
		var packageReferences = project.DependenciesReadOnly.Values
			.Where(x => x.ReferenceTypeReadOnly == ReferenceType.Package)
			.Select(x => x as PackageReference);

		var cachedPackageReferences = new List<CachedPackageReference>();

		foreach (var packageReference in packageReferences)
		{
			if (packageReference is null)
			{
				continue;
			}

			cachedPackageReferences.Add(new CachedPackageReference
			{
				Using = GetReferencedProjectId(packageReference),
				UsedProjectName = packageReference.UsedBy.Name,
				UsedBy = cachedProject.Id,
				Version = packageReference.Version
			});
		}

		return cachedPackageReferences;
	}


	private Guid GetReferencedProjectId(IReference reference)
	{
		var cachedProjectId = _cachedProjectProvider.GetCachedProjectId(reference.Using.Name);
		if (cachedProjectId is not null)
		{
			return (Guid)cachedProjectId;
		}

		var referencedCachedProject = GetCachedProject(reference.Using);

		return referencedCachedProject.Id;
	}
}