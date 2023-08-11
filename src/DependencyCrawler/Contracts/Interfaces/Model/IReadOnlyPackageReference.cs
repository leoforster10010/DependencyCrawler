namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyPackageReference : IReadOnlyReference
{
	public string? VersionReadOnly { get; }
}