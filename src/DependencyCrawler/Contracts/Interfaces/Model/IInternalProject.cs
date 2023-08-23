namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IInternalProject : IProject, IReadOnlyInternalProject
{
    public IDictionary<Guid, IProjectReference> ProjectReferences { get; set; }
}