namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyInternalProject : IReadOnlyProject
{
	public IReadOnlyDictionary<Guid, IReadOnlyPackageReference> PackageReferencesReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyProjectReference> ProjectReferencesReadOnly { get; }
}