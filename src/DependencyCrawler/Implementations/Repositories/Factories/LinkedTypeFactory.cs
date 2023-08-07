using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Implementations.Repositories.Factories;

public class LinkedTypeFactory : ILinkedTypeFactory
{
	public PackageReference GetPackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject)
	{
		var packageReference = new PackageReference
		{
			Version = packageReferenceInfo.Version,
			Using = referencedProject,
			UsedBy = parentProject
		};
		referencedProject.ReferencedBy.TryAdd(packageReference.UsedBy.Name, packageReference);

		return packageReference;
	}

	public PackageReference GetPackageReference(CachedPackageReference cachedPackageReference, IProject parentProject,
		IProject referencedProject)
	{
		var packageReference = new PackageReference
		{
			Version = cachedPackageReference.Version,
			Using = referencedProject,
			UsedBy = parentProject
		};
		referencedProject.ReferencedBy.TryAdd(packageReference.UsedBy.Name, packageReference);

		return packageReference;
	}

	public ProjectReference GetProjectReference(IProject parentProject,
		IProject referencedProject)
	{
		var projectReference = new ProjectReference
		{
			Using = referencedProject,
			UsedBy = parentProject
		};
		referencedProject.ReferencedBy.TryAdd(projectReference.UsedBy.Name, projectReference);

		return projectReference;
	}

	public ProjectNamespace GetProjectNamespace(NamespaceInfo namespaceInfo, IProject parentProject)
	{
		var projectNamespace = new ProjectNamespace
		{
			Name = namespaceInfo.Name,
			ParentProject = parentProject
		};

		foreach (var typeInfo in namespaceInfo.Types)
		{
			var namespaceType = GetNamespaceType(typeInfo, projectNamespace);
			projectNamespace.NamespaceTypes.TryAdd(namespaceType.FullName, namespaceType);
		}

		return projectNamespace;
	}

	public ProjectNamespace GetProjectNamespace(CachedProjectNamespace cachedProjectNamespace, IProject parentProject)
	{
		var projectNamespace = new ProjectNamespace
		{
			Name = cachedProjectNamespace.Name,
			ParentProject = parentProject
		};

		foreach (var typeInfo in cachedProjectNamespace.NamespaceTypes)
		{
			var namespaceType = GetNamespaceType(typeInfo, projectNamespace);
			projectNamespace.NamespaceTypes.TryAdd(namespaceType.FullName, namespaceType);
		}

		return projectNamespace;
	}

	private NamespaceType GetNamespaceType(TypeInfo typeInfo, IProjectNamespace parentNamespace)
	{
		var namespaceType = new NamespaceType
		{
			Name = typeInfo.Name,
			ParentNamespace = parentNamespace
		};

		foreach (var usingDirectiveInfo in typeInfo.UsingDirectives)
		{
			var typeUsingDirective = GetTypeUsingDirective(usingDirectiveInfo, namespaceType);
			namespaceType.UsingDirectives.TryAdd(typeUsingDirective.Name, typeUsingDirective);
		}

		return namespaceType;
	}

	private NamespaceType GetNamespaceType(CachedNamespaceType cachedNamespaceType, IProjectNamespace parentNamespace)
	{
		var namespaceType = new NamespaceType
		{
			Name = cachedNamespaceType.Name,
			ParentNamespace = parentNamespace
		};

		foreach (var usingDirectiveInfo in cachedNamespaceType.UsingDirectives)
		{
			var typeUsingDirective = GetTypeUsingDirective(usingDirectiveInfo, namespaceType);
			namespaceType.UsingDirectives.TryAdd(typeUsingDirective.Name, typeUsingDirective);
		}

		return namespaceType;
	}

	private TypeUsingDirective GetTypeUsingDirective(UsingDirectiveInfo usingDirectiveInfo, INamespaceType parentType)
	{
		var typeUsingDirective = new TypeUsingDirective
		{
			ReferencedNamespace = new UnresolvedNamespace
			{
				Name = usingDirectiveInfo.Namespace
			},
			ParentType = parentType,
			State = TypeUsingDirectiveState.Unlinked
		};

		return typeUsingDirective;
	}

	private TypeUsingDirective GetTypeUsingDirective(CachedTypeUsingDirective cachedTypeUsingDirective,
		INamespaceType parentType)
	{
		var typeUsingDirective = new TypeUsingDirective
		{
			ReferencedNamespace = new UnresolvedNamespace
			{
				Name = cachedTypeUsingDirective.Name
			},
			ParentType = parentType,
			State = TypeUsingDirectiveState.Unlinked
		};

		return typeUsingDirective;
	}
}