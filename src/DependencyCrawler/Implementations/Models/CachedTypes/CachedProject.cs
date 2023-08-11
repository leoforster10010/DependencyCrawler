using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedProject : Entity
{
	public required string Name { get; init; }
	public required ProjectType ProjectType { get; init; }

	public IList<CachedPackageReference> PackageReferences { get; set; } =
		new List<CachedPackageReference>();

	public IList<CachedProjectReference> ProjectReferences { get; set; } =
		new List<CachedProjectReference>();

	public IList<CachedProjectNamespace> Namespaces { get; set; } =
		new List<CachedProjectNamespace>();
}