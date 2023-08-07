using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface ILinkedTypeFactory
{
	PackageReference GetPackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject);

	ProjectReference GetProjectReference(IProject parentProject,
		IProject referencedProject);

	ProjectNamespace GetProjectNamespace(NamespaceInfo namespaceInfo, IProject parentProject);

	PackageReference GetPackageReference(CachedPackageReference cachedPackageReference, IProject parentProject,
		IProject referencedProject);

	ProjectNamespace GetProjectNamespace(CachedProjectNamespace cachedProjectNamespace, IProject parentProject);
}