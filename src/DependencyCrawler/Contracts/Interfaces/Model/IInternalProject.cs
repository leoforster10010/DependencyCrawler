namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IInternalProject : IProject, IReadOnlyInternalProject
{
	public IDictionary<string, IPackageReference> PackageReferences { get; set; }
	public IDictionary<string, IProjectReference> ProjectReferences { get; set; }
}