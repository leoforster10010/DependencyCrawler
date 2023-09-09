using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Data.Contracts.Entities;
using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class ExternalProject : Entity, IExternalProject
{
	//ToDo: if InternalProject with same Name exists, link to it
	private int? _dependencyLayer;
	private int? _dependencyLayerInternal;
	private int? _referenceLayer;

	public IDictionary<Guid, IPackageReference> PackageReferences { get; set; } =
		new Dictionary<Guid, IPackageReference>();

	public required string Name { get; init; }

	public int ReferenceLayer
	{
		get
		{
			_referenceLayer ??= References.Any() ? References.Values.Max(x => x.UsedBy.ReferenceLayer) + 1 : 0;

			return (int)_referenceLayer;
		}
	}

	public int DependencyLayer
	{
		get
		{
			_dependencyLayer ??= Dependencies.Any() ? Dependencies.Values.Max(x => x.Using.DependencyLayer) + 1 : 0;

			return (int)_dependencyLayer;
		}
	}

	public int DependencyLayerInternal
	{
		get
		{
			_dependencyLayerInternal ??= Dependencies.Any(x => x.Value.Using.ProjectType is ProjectType.Internal)
				? Dependencies.Values.Where(x => x.Using.ProjectType is ProjectType.Internal)
					.Max(x => x.Using.DependencyLayerInternal) + 1
				: 0;

			return (int)_dependencyLayerInternal;
		}
	}

	public IDictionary<Guid, IReference> Dependencies
	{
		get
		{
			var allDependencies = new Dictionary<Guid, IReference>();
			foreach (var packageReference in PackageReferences)
			{
				allDependencies.TryAdd(packageReference.Key, packageReference.Value);
			}

			return allDependencies;
		}
	}

	public IDictionary<Guid, IReference> References { get; set; } = new Dictionary<Guid, IReference>();

	public ProjectType ProjectType => ProjectType.External;

	public IDictionary<Guid, IProjectNamespace> Namespaces { get; set; } =
		new Dictionary<Guid, IProjectNamespace>();

	public IDictionary<Guid, INamespaceType> Types
	{
		get
		{
			var projectTypes = new Dictionary<Guid, INamespaceType>();
			var types = Namespaces.Values.SelectMany(x => x.NamespaceTypes.Values);
			foreach (var type in types)
			{
				projectTypes.TryAdd(type.Id, type);
			}

			return projectTypes;
		}
	}

	public IDictionary<Guid, ITypeUsingDirective> UsingDirectives
	{
		get
		{
			var projectUsingDirectives = new Dictionary<Guid, ITypeUsingDirective>();
			var usingDirectives = Namespaces.Values.SelectMany(x => x.TypeUsingDirectives.Values);
			foreach (var usingDirective in usingDirectives)
			{
				projectUsingDirectives.TryAdd(usingDirective.Id, usingDirective);
			}

			return projectUsingDirectives;
		}
	}

	public string NameReadOnly => Name;
	public ProjectType ProjectTypeReadOnly => ProjectType;
	public int ReferenceLayerReadOnly => ReferenceLayer;
	public int DependencyLayerReadOnly => DependencyLayer;
	public int DependencyLayerInternalReadOnly => DependencyLayerInternal;

	public IReadOnlyDictionary<Guid, IReadOnlyReference> DependenciesReadOnly =>
		Dependencies.ToDictionary(x => x.Key, x => x.Value as IReadOnlyReference);

	public IReadOnlyDictionary<Guid, IReadOnlyReference> ReferencesReadOnly =>
		References.ToDictionary(x => x.Key, x => x.Value as IReadOnlyReference);

	public IReadOnlyDictionary<Guid, IReadOnlyProjectNamespace> NamespacesReadOnly =>
		Namespaces.ToDictionary(x => x.Key, x => x.Value as IReadOnlyProjectNamespace);

	public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> TypesReadOnly =>
		Types.ToDictionary(x => x.Key, x => x.Value as IReadOnlyNamespaceType);

	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
		UsingDirectives.ToDictionary(x => x.Key, x => x.Value as IReadOnlyTypeUsingDirective);

	public IReadOnlyDictionary<Guid, IReadOnlyPackageReference> PackageReferencesReadOnly =>
		PackageReferences.ToDictionary(x => x.Key, x => x.Value as IReadOnlyPackageReference);
}