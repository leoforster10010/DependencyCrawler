using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Contracts.Interfaces.Model;

public interface IReadOnlyNamespaceType : IEntity
{
	public string NameReadOnly { get; }
	public string FullNameReadOnly { get; }
	public FileType FileTypeReadOnly { get; }
	public IReadOnlyProjectNamespace ParentNamespaceReadOnly { get; }
	public IReadOnlyProject ParentProjectReadOnly { get; }
	public IReadOnlyDictionary<Guid, IReadOnlyTypeUsingDirective> UsingDirectivesReadOnly { get; }
}