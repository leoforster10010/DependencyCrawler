using DependencyCrawler.Data.Contracts.Entities;

namespace DependencyCrawler.Data.Contracts.Interfaces;

public interface IEntity
{
	Guid Id { get; init; }
	bool Equals(Entity? other);
	bool Equals(object? obj);
	int GetHashCode();
}