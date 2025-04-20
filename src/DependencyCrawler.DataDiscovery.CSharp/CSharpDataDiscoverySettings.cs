using System.ComponentModel.DataAnnotations;

namespace DependencyCrawler.DataDiscovery.CSharp;

internal class CSharpDataDiscoverySettings
{
	[Required] public required string RootDirectory { get; init; }
	[Required] public required string CsProjSearchPattern { get; init; }
	[Required] public required string DllSearchPattern { get; init; }
	[Required] public required string PackageReferenceIdentifier { get; init; }
	[Required] public required string ProjectReferenceIdentifier { get; init; }
}