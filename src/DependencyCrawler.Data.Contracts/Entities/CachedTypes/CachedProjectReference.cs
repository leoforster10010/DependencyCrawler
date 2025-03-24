using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Entities.CachedTypes;

public class CachedProjectReference : Entity
{
	public required Guid Using { get; set; }
	public required string UsedProjectName { get; set; }
	public required Guid UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Project;
}