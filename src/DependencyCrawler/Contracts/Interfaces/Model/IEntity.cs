using DependencyCrawler.Implementations.Models.LinkedTypes;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IEntity
{
    Guid Id { get; init; }
    bool Equals(Entity? other);
    bool Equals(object? obj);
    int GetHashCode();
}