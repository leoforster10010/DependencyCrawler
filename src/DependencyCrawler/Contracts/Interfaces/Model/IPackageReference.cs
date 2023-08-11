namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IPackageReference : IReference, IReadOnlyPackageReference
{
	public string? Version { get; set; }
}