using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedPackageReference
{
	public required Guid Using { get; set; }
	public required string UsedProjectName { get; set; }
	public required Guid UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Package;
	public string? Version { get; set; }
}