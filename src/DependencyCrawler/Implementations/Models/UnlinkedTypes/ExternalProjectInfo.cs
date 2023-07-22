using System.Reflection;

namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

public class ExternalProjectInfo
{
	public required string Name { get; set; }
	public required Assembly? Assembly { get; set; }
	public static bool External => true;
	public IList<NamespaceInfo> Namespaces { get; set; } = new List<NamespaceInfo>();
	public IList<PackageReferenceInfo> PackageReferences { get; set; } = new List<PackageReferenceInfo>();
}