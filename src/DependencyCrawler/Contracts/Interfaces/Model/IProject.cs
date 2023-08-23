using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

internal interface IProject : IReadOnlyProject
{
    public string Name { get; init; }
    public ProjectType ProjectType { get; }
    public IDictionary<Guid, IPackageReference> PackageReferences { get; set; }
    public IDictionary<Guid, IReference> Dependencies { get; }
    public IDictionary<Guid, IReference> ReferencedBy { get; set; }
    public IDictionary<Guid, IProjectNamespace> Namespaces { get; set; }
    public IDictionary<Guid, INamespaceType> Types { get; }
    public IDictionary<Guid, ITypeUsingDirective> UsingDirectives { get; }
}