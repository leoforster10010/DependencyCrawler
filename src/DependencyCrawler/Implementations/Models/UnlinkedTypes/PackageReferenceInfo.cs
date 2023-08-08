namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

internal class PackageReferenceInfo
{
	public required string Using { get; set; }
	public required string UsedBy { get; set; }
	public string? Version { get; set; }
}