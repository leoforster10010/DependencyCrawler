using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Implementations.Data.Enum;
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

	public ProjectReference GetProjectReference(ProjectReferenceInfo projectReferenceInfo, IProject parentProject,
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
}