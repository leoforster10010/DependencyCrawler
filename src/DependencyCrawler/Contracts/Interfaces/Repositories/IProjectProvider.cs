using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface IProjectProvider : IReadOnlyProjectProvider
{
	IDictionary<string, InternalProject> InternalProjects { get; }
	IDictionary<string, ExternalProject> ExternalProjects { get; }
	IDictionary<string, IProject> AllProjects { get; }
	void AddInternalProject(InternalProject internalProject);
	void AddExternalProject(ExternalProject externalProject);
	void AddUnresolvedProject(UnresolvedProject unresolvedProject);
	void Clear();
}