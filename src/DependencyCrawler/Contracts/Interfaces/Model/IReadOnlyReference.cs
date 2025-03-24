using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyReference : IEntity
{
	public IReadOnlyProject UsingReadOnly { get; }
	public IReadOnlyProject UsedByReadOnly { get; }
	public ReferenceType ReferenceTypeReadOnly { get; }
}