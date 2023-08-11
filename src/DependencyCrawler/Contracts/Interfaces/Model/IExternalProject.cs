namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IExternalProject : IProject, IReadOnlyExternalProject
{
	public IDictionary<string, IPackageReference> PackageReferences { get; set; }
}