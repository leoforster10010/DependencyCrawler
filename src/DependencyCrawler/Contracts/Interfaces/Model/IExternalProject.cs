namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IExternalProject : IProject, IReadOnlyExternalProject
{
	public IDictionary<Guid, IPackageReference> PackageReferences { get; set; }
}