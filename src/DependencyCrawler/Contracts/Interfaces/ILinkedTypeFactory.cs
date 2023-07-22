using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces;

public interface ILinkedTypeFactory
{
	PackageReference GetPackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject);

	ProjectReference GetProjectReference(ProjectReferenceInfo projectReferenceInfo, IProject parentProject,
		IProject referencedProject);

	ProjectNamespace GetProjectNamespace(NamespaceInfo namespaceInfo, IProject parentProject);
}