using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models.LinkedTypes;

internal class UnresolvedProject : Entity, IProject
{
    public string Name { get; init; } = string.Empty;
    public ProjectType ProjectType => ProjectType.Unresolved;

    public IDictionary<Guid, IPackageReference> PackageReferences { get; set; } =
        new Dictionary<Guid, IPackageReference>();

    public IDictionary<Guid, IReference> Dependencies => new Dictionary<Guid, IReference>();
    public IDictionary<Guid, IReference> ReferencedBy { get; set; } = new Dictionary<Guid, IReference>();

    public IDictionary<Guid, IProjectNamespace> Namespaces { get; set; } =
        new Dictionary<Guid, IProjectNamespace>();

    public IDictionary<Guid, INamespaceType> Types => new Dictionary<Guid, INamespaceType>();
    public IDictionary<Guid, ITypeUsingDirective> UsingDirectives => new Dictionary<Guid, ITypeUsingDirective>();
    public string NameReadOnly => Name;
    public ProjectType ProjectTypeReadOnly => ProjectType;

    public IReadOnlyDictionary<Guid, IReadOnlyReference> DependenciesReadOnly =>
        new Dictionary<Guid, IReadOnlyReference>();

    public IReadOnlyDictionary<Guid, IReadOnlyReference> ReferencedByReadOnly =>
        new Dictionary<Guid, IReadOnlyReference>();

    public IReadOnlyDictionary<Guid, IReadOnlyProjectNamespace> NamespacesReadOnly =>
        new Dictionary<Guid, IReadOnlyProjectNamespace>();

    public IReadOnlyDictionary<Guid, IReadOnlyNamespaceType> TypesReadOnly =>
        new Dictionary<Guid, IReadOnlyNamespaceType>();

    public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly =>
        new Dictionary<Guid, IReadOnlyTypeUsingDirective>();
}

public interface IEntity
{
    Guid Id { get; init; }
    bool Equals(Entity? other);
    bool Equals(object? obj);
    int GetHashCode();
}

public abstract class Entity : IEquatable<Entity>, IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity entity)
        {
            return false;
        }


        return entity.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Entity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (other.GetType() != GetType())
        {
            return false;
        }

        return other.Id == Id;
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return left is not null && right is not null && left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}