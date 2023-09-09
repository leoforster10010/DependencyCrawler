using DependencyCrawler.Implementations.Data.Enum;
using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Implementations.Models.CachedTypes;

public class CachedProjectReference : Entity
{
	public required Guid Using { get; set; }
	public required string UsedProjectName { get; set; }
	public required Guid UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Project;
}