namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyInternalProject : IReadOnlyProject
{
	public IReadOnlyDictionary<string, IReadOnlyPackageReference> PackageReferencesReadOnly { get; }
	public IReadOnlyDictionary<string, IReadOnlyProjectReference> ProjectReferencesReadOnly { get; }
}