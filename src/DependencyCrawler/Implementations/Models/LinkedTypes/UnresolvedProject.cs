using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedProject : Entity, IProject
{
	public string Name { get; init; } = string.Empty;
	public ProjectType ProjectType => ProjectType.Unresolved;
	public int ReferenceLayer => 0;
	public int DependencyLayer => 0;
	public int DependencyLayerInternal => 0;

	public IDictionary<Guid, IPackageReference> PackageReferences { get; set; } =
		new Dictionary<Guid, IPackageReference>();

	public IDictionary<Guid, IReference> Dependencies => new Dictionary<Guid, IReference>();
	public IDictionary<Guid, IReference> References { get; set; } = new Dictionary<Guid, IReference>();

	public IDictionary<Guid, IProjectNamespace> Namespaces { get; set; } =
		new Dictionary<Guid, IProjectNamespace>();

	public IDictionary<Guid, INamespaceType> Types => new Dictionary<Guid, INamespaceType>();
	public IDictionary<Guid, ITypeUsingDirective> UsingDirectives => new Dictionary<Guid, ITypeUsingDirective>();
	public string NameReadOnly => Name;
	public ProjectType ProjectTypeReadOnly => ProjectType;
	public int ReferenceLayerReadOnly => ReferenceLayer;
	public int DependencyLayerReadOnly => DependencyLayer;
	public int DependencyLayerInternalReadOnly => DependencyLayerInternal;

	public IReadOnlyDictionary<Guid, IReadOnlyReference> DependenciesReadOnly =>
		new Dictionary<Guid, IReadOnlyReference>();

	public IReadOnlyDictionary<Guid, IReadOnlyReference> ReferencesReadOnly =>
		new Dictionary<Guid, IReadOnlyReference>();

	public IReadOnlyDictionary<Guid, IReadOnlyProjectNamespace> NamespacesReadOnly =>
		new Dictionary<Guid, IReadOnlyProjectNamespace>();

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> TypesReadOnly =>
		new Dictionary<Guid, IReadOnlyNamespaceType>();

	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		new Dictionary<Guid, IReadOnlyTypeUsingDirective>();
}