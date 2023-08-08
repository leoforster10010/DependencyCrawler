using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface IProjectProvider
{
	IEnumerable<InternalProject> InternalProjects { get; }
	IEnumerable<ExternalProject> ExternalProjects { get; }
	IDictionary<string, IProject> AllProjects { get; }
	void AddInternalProject(InternalProject internalProject);
	void AddExternalProject(ExternalProject externalProject);
	void AddUnresolvedProject(string name);
	void Clear();
}