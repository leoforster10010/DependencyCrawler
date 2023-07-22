using System.Reflection;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces;

public interface IExternalProjectInfoLoader
{
	IList<NamespaceInfo> LoadNamespaces(Assembly assembly);
}