namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

internal class NamespaceInfo
{
	public required string Name { get; set; }
	public IList<TypeInfo> Types { get; set; } = new List<TypeInfo>();
}