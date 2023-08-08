using System.Reflection;
using DependencyCrawler.Implementations.Models.UnlinkedTypes;

namespace DependencyCrawler.Framework.Extensions;

internal static class AssemblyExtensions
{
	public static string GetProjectName(this Assembly assembly)
	{
		//ToDo test
		return assembly.GetName().Name!;
	}

	public static IEnumerable<PackageReferenceInfo> GetPackageReferenceInfos(this Assembly assembly)
	{
		var referencedAssemblies = assembly.GetReferencedAssemblies();
		var packageReferenceInfos = referencedAssemblies.Select(x => new PackageReferenceInfo
		{
			Version = x.Version is not null
				? $"{x.Version.Major}.{x.Version.Minor}.{x.Version.Build}"
				: null,
			Using = x.Name ?? "",
			UsedBy = assembly.GetProjectName()
		}).Distinct();
		return packageReferenceInfos;
	}

	public static IEnumerable<NamespaceInfo> GetNamespaces(this Assembly assembly)
	{
		var typeNamespaces = assembly.GetTypesSafe().Select(x => x.Namespace);

		var namespaces = typeNamespaces.Distinct().Select(x => new NamespaceInfo
		{
			Name = x!
		});

		return namespaces;
	}

	public static IEnumerable<Type> GetTypesSafe(this Assembly assembly)
	{
		List<Type> types;
		try
		{
			types = assembly.GetTypes().ToList();
		}
		catch (ReflectionTypeLoadException ex)
		{
			types = ex.Types.Where(x => x is not null).ToList()!;
		}

		var safeTypes = new List<Type>();
		foreach (var type in types.Where(x => !x.Name.StartsWith("<")))
		{
			try
			{
				if (type.Namespace is not null)
				{
					safeTypes.Add(type);
				}
			}
			catch (Exception)
			{
				// ignored
			}
		}

		return safeTypes;
	}
}