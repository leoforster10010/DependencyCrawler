using System.Reflection;
using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Framework.Extensions;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;
using TypeInfo = DependencyCrawler.Implementations.Models.UnlinkedTypes.TypeInfo;

namespace DependencyCrawler.Implementations.Repositories.Loader;

public class ExternalProjectInfoLoader : IExternalProjectInfoLoader
{
	public IList<NamespaceInfo> LoadNamespaces(Assembly assembly)
	{
		var namespaces = assembly.GetNamespaces().ToList();
		foreach (var ns in namespaces)
		{
			ns.Types = LoadTypeInfos(assembly, ns);
		}

		return namespaces;
	}

	private IList<TypeInfo> LoadTypeInfos(Assembly assembly, NamespaceInfo ns)
	{
		var types = assembly.GetTypesSafe().Where(x => x.Namespace == ns.Name).Select(x => new TypeInfo
		{
			Name = x.Name
		}).ToList();
		return types;
	}
}