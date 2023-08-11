namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyExternalProject : IReadOnlyProject
{
	public IReadOnlyDictionary<string, IReadOnlyPackageReference> PackageReferencesReadOnly { get; }
}