using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedProject
{
	public Guid Id { get; } = Guid.NewGuid();
	public required string Name { get; init; }
	public required ProjectType ProjectType { get; init; }

	public IList<CachedPackageReference> PackageReferences { get; set; } =
		new List<CachedPackageReference>();

	public IList<CachedProjectReference> ProjectReferences { get; set; } =
		new List<CachedProjectReference>();

	public IList<CachedProjectNamespace> Namespaces { get; set; } =
		new List<CachedProjectNamespace>();
}