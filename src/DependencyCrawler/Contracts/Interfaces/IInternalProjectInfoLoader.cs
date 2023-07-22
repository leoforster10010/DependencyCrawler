using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IInternalProjectInfoLoader
{
	IEnumerable<NamespaceInfo> LoadNamespaces(string projectName);
}