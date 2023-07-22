namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

public class ProjectReferenceInfo
{
	public required string Using { get; set; }
	public required string UsedBy { get; set; }
}