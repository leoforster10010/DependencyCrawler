using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IReadOnlyProjectProvider
{
	IReadOnlyDictionary<string, IReadOnlyInternalProject> InternalProjectsReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyExternalProject> ExternalProjectsReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyProject> AllProjectsReadOnly { get; }
}