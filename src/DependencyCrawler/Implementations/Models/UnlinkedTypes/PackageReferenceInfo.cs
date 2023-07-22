namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

public class PackageReferenceInfo
{
	public required string Using { get; set; }
	public required string UsedBy { get; set; }
	public string? Version { get; set; }
}