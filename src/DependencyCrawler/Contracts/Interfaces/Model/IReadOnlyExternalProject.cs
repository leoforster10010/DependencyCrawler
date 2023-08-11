namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyExternalProject : IReadOnlyProject
{
	public IReadOnlyDictionary<Guid, IReadOnlyPackageReference> PackageReferencesReadOnly { get; }
}