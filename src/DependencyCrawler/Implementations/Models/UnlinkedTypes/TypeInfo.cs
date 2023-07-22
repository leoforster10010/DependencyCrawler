namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

public class TypeInfo
{
	public required string Name { get; set; }
	public IList<UsingDirectiveInfo> UsingDirectives { get; set; } = new List<UsingDirectiveInfo>();
}