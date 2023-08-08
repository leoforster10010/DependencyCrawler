using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

internal interface IInternalProjectInfoLoader
{
	IEnumerable<NamespaceInfo> LoadNamespaces(string projectName);
}