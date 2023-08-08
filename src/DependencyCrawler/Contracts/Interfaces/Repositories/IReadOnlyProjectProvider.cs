using DependencyCrawler.Contracts.Interfaces.Model;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IReadOnlyProjectProvider
{
	IReadOnlyList<IReadOnlyProject> InternalProjectsReadOnly { get; }
	IReadOnlyList<IReadOnlyProject> ExternalProjectsReadOnly { get; }
	IReadOnlyDictionary<string, IReadOnlyProject> AllProjectsReadOnly { get; }
}