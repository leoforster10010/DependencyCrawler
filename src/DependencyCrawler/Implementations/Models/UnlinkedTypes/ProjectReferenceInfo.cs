namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

internal class ProjectReferenceInfo
{
	public required string Using { get; set; }
	public required string UsedBy { get; set; }
}