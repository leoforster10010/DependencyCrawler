using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Implementations.Repositories.Factories;

internal class LinkedTypeFactory : ILinkedTypeFactory
{
	public InternalProject CreateInternalProject(CachedProject cachedProject)
	{
		if (cachedProject.ProjectType is not ProjectType.Internal)
		{
			throw new Exception("Wrong ProjectType, can't create an InternalProject out of this type");
		}

		var internalProject = new InternalProject
		{
			Name = cachedProject.Name,
			Id = cachedProject.Id
		};

		foreach (var cachedProjectNamespace in cachedProject.Namespaces)
		{
			var projectNamespace = GetProjectNamespace(cachedProjectNamespace, internalProject);
			internalProject.Namespaces.TryAdd(projectNamespace.Id, projectNamespace);
		}

		return internalProject;
	}

	public ExternalProject CreateExternalProject(CachedProject cachedProject)
	{
		if (cachedProject.ProjectType is not ProjectType.External)
		{
			throw new Exception("Wrong ProjectType, can't create an ExternalProject out of this type");
		}

		var externalProject = new ExternalProject
		{
			Name = cachedProject.Name,
			Id = cachedProject.Id
		};

		foreach (var cachedProjectNamespace in cachedProject.Namespaces)
		{
			var projectNamespace = GetProjectNamespace(cachedProjectNamespace, externalProject);
			externalProject.Namespaces.TryAdd(projectNamespace.Id, projectNamespace);
		}

		return externalProject;
	}

	public UnresolvedProject CreateUnresolvedProject(CachedProject cachedProject)
	{
		if (cachedProject.ProjectType is not ProjectType.Unresolved)
		{
			throw new Exception("Wrong ProjectType, can't create an UnresolvedProject out of this type");
		}

		return new UnresolvedProject
		{
			Name = cachedProject.Name,
			Id = cachedProject.Id
		};
	}

	public InternalProject CreateInternalProject(InternalProjectInfo internalProjectInfo)
	{
		var internalProject = new InternalProject
		{
			Name = internalProjectInfo.Name
		};

		foreach (var namespaceInfo in internalProjectInfo.Namespaces)
		{
			var projectNamespace = GetProjectNamespace(namespaceInfo, internalProject);
			internalProject.Namespaces.TryAdd(projectNamespace.Id, projectNamespace);
		}

		//Adding RootNamespace
		var rootNamespace = new ProjectNamespace
		{
			Name = internalProject.Name,
			ParentProject = internalProject
		};
		internalProject.Namespaces.TryAdd(rootNamespace.Id, rootNamespace);

		return internalProject;
	}

	public ExternalProject CreateExternalProject(ExternalProjectInfo externalProjectInfo)
	{
		var externalProject = new ExternalProject
		{
			Name = externalProjectInfo.Name
		};

		foreach (var namespaceInfo in externalProjectInfo.Namespaces)
		{
			var projectNamespace = GetProjectNamespace(namespaceInfo, externalProject);
			externalProject.Namespaces.TryAdd(projectNamespace.Id, projectNamespace);
		}

		//Adding RootNamespace
		var rootNamespace = new ProjectNamespace
		{
			Name = externalProject.Name,
			ParentProject = externalProject
		};
		externalProject.Namespaces.TryAdd(rootNamespace.Id, rootNamespace);

		return externalProject;
	}

	public UnresolvedProject CreateUnresolvedProject(string name)
	{
		return new UnresolvedProject
		{
			Name = name
		};
	}

	public PackageReference GetPackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject)
	{
		var packageReference = new PackageReference
		{
			Version = packageReferenceInfo.Version,
			Using = referencedProject,
			UsedBy = parentProject
		};
		referencedProject.ReferencedBy.TryAdd(packageReference.Id, packageReference);

		return packageReference;
	}

	public PackageReference GetPackageReference(CachedPackageReference cachedPackageReference, IProject parentProject,
		IProject referencedProject)
	{
		var packageReference = new PackageReference
		{
			Version = cachedPackageReference.Version,
			Using = referencedProject,
			UsedBy = parentProject,
			Id = cachedPackageReference.Id
		};
		referencedProject.ReferencedBy.TryAdd(packageReference.Id, packageReference);

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
		referencedProject.ReferencedBy.TryAdd(projectReference.Id, projectReference);

		return projectReference;
	}

	public ProjectReference GetProjectReference(CachedProjectReference cachedProjectReference, IProject parentProject,
		IProject referencedProject)
	{
		var projectReference = new ProjectReference
		{
			Using = referencedProject,
			UsedBy = parentProject,
			Id = cachedProjectReference.Id
		};
		referencedProject.ReferencedBy.TryAdd(projectReference.Id, projectReference);

		return projectReference;
	}

	private ProjectNamespace GetProjectNamespace(NamespaceInfo namespaceInfo, IProject parentProject)
	{
		var projectNamespace = new ProjectNamespace
		{
			Name = namespaceInfo.Name,
			ParentProject = parentProject
		};

		foreach (var typeInfo in namespaceInfo.Types)
		{
			var namespaceType = GetNamespaceType(typeInfo, projectNamespace);
			projectNamespace.NamespaceTypes.TryAdd(namespaceType.Id, namespaceType);
		}

		return projectNamespace;
	}

	private ProjectNamespace GetProjectNamespace(CachedProjectNamespace cachedProjectNamespace, IProject parentProject)
	{
		var projectNamespace = new ProjectNamespace
		{
			Name = cachedProjectNamespace.Name,
			ParentProject = parentProject,
			Id = cachedProjectNamespace.Id
		};

		foreach (var typeInfo in cachedProjectNamespace.NamespaceTypes)
		{
			var namespaceType = GetNamespaceType(typeInfo, projectNamespace);
			projectNamespace.NamespaceTypes.TryAdd(namespaceType.Id, namespaceType);
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
			namespaceType.UsingDirectives.TryAdd(typeUsingDirective.Id, typeUsingDirective);
		}

		return namespaceType;
	}

	private NamespaceType GetNamespaceType(CachedNamespaceType cachedNamespaceType, IProjectNamespace parentNamespace)
	{
		var namespaceType = new NamespaceType
		{
			Name = cachedNamespaceType.Name,
			ParentNamespace = parentNamespace,
			Id = cachedNamespaceType.Id
		};

		foreach (var usingDirectiveInfo in cachedNamespaceType.UsingDirectives)
		{
			var typeUsingDirective = GetTypeUsingDirective(usingDirectiveInfo, namespaceType);
			namespaceType.UsingDirectives.TryAdd(typeUsingDirective.Id, typeUsingDirective);
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
				Name = cachedTypeUsingDirective.Name,
				Id = cachedTypeUsingDirective.ReferencedNamespaceId
			},
			ParentType = parentType,
			State = TypeUsingDirectiveState.Unlinked
		};

		return typeUsingDirective;
	}
}