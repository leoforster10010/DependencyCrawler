using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.UnlinkedTypes;

internal class TypeInfo
{
	public required string Name { get; set; }
	public required FileType FileType { get; init; }
	public IList<UsingDirectiveInfo> UsingDirectives { get; set; } = new List<UsingDirectiveInfo>();
	public string FileName { get; set; } = string.Empty;
}