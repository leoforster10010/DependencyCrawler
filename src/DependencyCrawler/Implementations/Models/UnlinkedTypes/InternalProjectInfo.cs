using Microsoft.Build.Construction;

namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

public class InternalProjectInfo
{
	public required string Name { get; set; }
	public required ProjectRootElement ProjectRootElement { get; set; }
	public static bool External => false;
	public IList<NamespaceInfo> Namespaces { get; set; } = new List<NamespaceInfo>();
	public IList<PackageReferenceInfo> PackageReferences { get; set; } = new List<PackageReferenceInfo>();
	public IList<ProjectReferenceInfo> ProjectReferences { get; set; } = new List<ProjectReferenceInfo>();
}