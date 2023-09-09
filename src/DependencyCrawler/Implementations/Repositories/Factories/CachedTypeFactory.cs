using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Entities.CachedTypes;
using DependencyCrawler.Data.Contracts.Enum;
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
			ProjectType = project.ProjectTypeReadOnly,
			Id = project.Id
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
				NamespaceTypes = GetCachedTypes(projectNamespace),
				Id = projectNamespace.Id
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
				UsingDirectives = GetCachedUsingDirectives(namespaceType),
				Id = namespaceType.Id
			});
		}

		return cachedNamespaceTypes;
	}

	private IList<CachedTypeUsingDirective> GetCachedUsingDirectives(IReadOnlyNamespaceType namespaceType)
	{
		var cachedTypeUsingDirectives = new List<CachedTypeUsingDirective>();

		var usingDirectives =
			namespaceType.UsingDirectivesReadOnly.Values.Where(x =>
				x.StateReadOnly is TypeUsingDirectiveState.Linked or TypeUsingDirectiveState.Unresolved);
		foreach (var typeUsingDirective in usingDirectives)
		{
			cachedTypeUsingDirectives.Add(new CachedTypeUsingDirective
			{
				Name = typeUsingDirective.NameReadOnly,
				State = typeUsingDirective.StateReadOnly,
				ReferencedNamespaceId = typeUsingDirective.ReferencedNamespaceReadOnly.Id,
				Id = typeUsingDirective.Id
			});
		}

		return cachedTypeUsingDirectives;
	}


	private IList<CachedProjectReference> GetCachedProjectReferences(CachedProject cachedProject,
		IReadOnlyProject project)
	{
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
				Using = projectReference.Using.Id,
				UsedProjectName = projectReference.Using.Name,
				UsedBy = cachedProject.Id,
				Id = projectReference.Id
			});
		}

		return cachedProjectReferences;
	}

	private IList<CachedPackageReference> GetCachedPackageReferences(CachedProject cachedProject,
		IReadOnlyProject project)
	{
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
				Using = packageReference.Using.Id,
				UsedProjectName = packageReference.Using.Name,
				UsedBy = cachedProject.Id,
				Version = packageReference.Version,
				Id = packageReference.Id
			});
		}

		return cachedPackageReferences;
	}
}