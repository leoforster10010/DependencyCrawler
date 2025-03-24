using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities.CachedTypes;
using DependencyCrawler.Implementations.Models.LinkedTypes;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface ILinkedTypeFactory
{
	PackageReference CreatePackageReference(PackageReferenceInfo packageReferenceInfo, IProject parentProject,
		IProject referencedProject);

	ProjectReference CreateProjectReference(IProject parentProject,
		IProject referencedProject);

	PackageReference CreatePackageReference(CachedPackageReference cachedPackageReference, IProject parentProject,
		IProject referencedProject);

	InternalProject CreateInternalProject(CachedProject cachedProject);
	ExternalProject CreateExternalProject(CachedProject cachedProject);
	UnresolvedProject CreateUnresolvedProject(CachedProject cachedProject);
	InternalProject CreateInternalProject(InternalProjectInfo internalProjectInfo);
	ExternalProject CreateExternalProject(ExternalProjectInfo externalProjectInfo);
	UnresolvedProject CreateUnresolvedProject(string name);

	ProjectReference CreateProjectReference(CachedProjectReference cachedProjectReference, IProject parentProject,
		IProject referencedProject);
}