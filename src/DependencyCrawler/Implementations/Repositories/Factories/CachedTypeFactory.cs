using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Repositories.Factories;

public class CachedTypeFactory : ICachedTypeFactory
{
	private readonly ICachedProjectProvider _cachedProjectProvider;

	public CachedTypeFactory(ICachedProjectProvider cachedProjectProvider)
	{
		_cachedProjectProvider = cachedProjectProvider;
	}

	public CachedProject GetCachedProject(IProject project)
	{
		var cachedProject = new CachedProject
		{
			Name = project.Name,
			ProjectType = project.ProjectType
		};

		cachedProject.PackageReferences = GetCachedPackageReferences(cachedProject, project);
		cachedProject.ProjectReferences = GetCachedProjectReferences(cachedProject, project);
		cachedProject.Namespaces = GetCachedNamespaces(project);

		return cachedProject;
	}

	private IList<CachedProjectNamespace> GetCachedNamespaces(IProject project)
	{
		var cachedProjectNamespaces = new List<CachedProjectNamespace>();

		foreach (var projectNamespace in project.Namespaces.Values)
		{
			cachedProjectNamespaces.Add(new CachedProjectNamespace
			{
				Name = projectNamespace.Name,
				NamespaceTypes = GetCachedTypes(projectNamespace)
			});
		}

		return cachedProjectNamespaces;
	}

	private IList<CachedNamespaceType> GetCachedTypes(IProjectNamespace projectNamespace)
	{
		var cachedNamespaceTypes = new List<CachedNamespaceType>();

		foreach (var namespaceType in projectNamespace.NamespaceTypes.Values)
		{
			cachedNamespaceTypes.Add(new CachedNamespaceType
			{
				Name = namespaceType.Name,
				UsingDirectives = GetCachedUsingDirectives(namespaceType)
			});
		}

		return cachedNamespaceTypes;
	}

	private IList<CachedTypeUsingDirective> GetCachedUsingDirectives(INamespaceType namespaceType)
	{
		var cachedTypeUsingDirectives = new List<CachedTypeUsingDirective>();

		var linkedUsingDirectives =
			namespaceType.UsingDirectives.Values.Where(x => x.State is TypeUsingDirectiveState.Linked);
		foreach (var typeUsingDirective in linkedUsingDirectives)
		{
			cachedTypeUsingDirectives.Add(new CachedTypeUsingDirective
			{
				Name = typeUsingDirective.Name,
				State = typeUsingDirective.State,
				ReferencedNamespaceId = GetCachedNamespaceId(typeUsingDirective.ReferencedNamespace)
			});
		}

		return cachedTypeUsingDirectives;
	}

	private Guid GetCachedNamespaceId(IProjectNamespace referencedNamespace)
	{
		var cachedNamespaceId = _cachedProjectProvider.GetCachedNamespaceId(referencedNamespace.Name);

		if (cachedNamespaceId is not null)
		{
			return (Guid)cachedNamespaceId;
		}

		var referencedCachedProject = GetCachedProject(referencedNamespace.ParentProject);
		_cachedProjectProvider.AddCachedProject(referencedCachedProject);

		return referencedCachedProject.Namespaces.First(x => x.Name == referencedNamespace.Name).Id;
	}


	private IList<CachedProjectReference> GetCachedProjectReferences(CachedProject cachedProject, IProject project)
	{
		var projectReferences = project.Dependencies.Values
			.Where(x => x.ReferenceType == ReferenceType.Project)
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
				UsedBy = cachedProject.Id
			});
		}

		return cachedProjectReferences;
	}

	private IList<CachedPackageReference> GetCachedPackageReferences(CachedProject cachedProject, IProject project)
	{
		var packageReferences = project.Dependencies.Values
			.Where(x => x.ReferenceType == ReferenceType.Package)
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
		_cachedProjectProvider.AddCachedProject(referencedCachedProject);

		return referencedCachedProject.Id;
	}
}