using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Models;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IProjectProvider
{
	IEnumerable<InternalProject> InternalProjects { get; }
	IEnumerable<ExternalProject> ExternalProjects { get; }
	IDictionary<string, IProject> AllProjects { get; }
	void AddInternalProject(InternalProject internalProject);
	void AddExternalProject(ExternalProject externalProject);
	void AddUnresolvedProject(string name);
}