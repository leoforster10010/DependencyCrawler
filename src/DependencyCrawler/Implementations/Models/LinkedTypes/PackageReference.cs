using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

public class PackageReference : IReference
{
	public string? Version { get; set; }
	public required IProject Using { get; set; }
	public required IProject UsedBy { get; set; }
	public ReferenceType ReferenceType => ReferenceType.Package;
}