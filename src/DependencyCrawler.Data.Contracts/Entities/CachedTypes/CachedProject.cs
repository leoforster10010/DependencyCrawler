using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Entities.CachedTypes;

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