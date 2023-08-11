namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IInternalProject : IProject, IReadOnlyInternalProject
{
	public IDictionary<Guid, IPackageReference> PackageReferences { get; set; }
	public IDictionary<Guid, IProjectReference> ProjectReferences { get; set; }
}