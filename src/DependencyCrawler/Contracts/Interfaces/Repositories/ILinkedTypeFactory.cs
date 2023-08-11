using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ILinkedTypeFactory
{
	PackageReference GetPackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject);

	ProjectReference GetProjectReference(IProject parentProject,
		IProject referencedProject);

	PackageReference GetPackageReference(CachedPackageReference cachedPackageReference, IProject parentProject,
		IProject referencedProject);

	InternalProject CreateInternalProject(CachedProject cachedProject);
	ExternalProject CreateExternalProject(CachedProject cachedProject);
	UnresolvedProject CreateUnresolvedProject(CachedProject cachedProject);
	InternalProject CreateInternalProject(InternalProjectInfo internalProjectInfo);
	ExternalProject CreateExternalProject(ExternalProjectInfo externalProjectInfo);
	UnresolvedProject CreateUnresolvedProject(string name);
}