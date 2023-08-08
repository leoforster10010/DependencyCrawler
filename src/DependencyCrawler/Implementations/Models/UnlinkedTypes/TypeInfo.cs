namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

internal class TypeInfo
{
	public required string Name { get; set; }
	public IList<UsingDirectiveInfo> UsingDirectives { get; set; } = new List<UsingDirectiveInfo>();
}