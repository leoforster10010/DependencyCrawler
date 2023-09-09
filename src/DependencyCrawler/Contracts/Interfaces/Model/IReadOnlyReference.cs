using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyReference : IEntity
{
    public IReadOnlyProject UsingReadOnly { get; }
    public IReadOnlyProject UsedByReadOnly { get; }
    public ReferenceType ReferenceTypeReadOnly { get; }
}