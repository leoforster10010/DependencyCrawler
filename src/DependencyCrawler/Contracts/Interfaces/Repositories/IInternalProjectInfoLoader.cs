using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IInternalProjectInfoLoader
{
	IEnumerable<NamespaceInfo> LoadNamespaces(string projectName);
}