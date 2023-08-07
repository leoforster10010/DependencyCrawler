using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedProjectReference
{
	public required Guid Using { get; set; }
	public required string UsedProjectName { get; set; }
	public required Guid UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Project;
}