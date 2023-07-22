using DependencyCrawler.Contracts.Interfaces.Model;
using DependencyCrawler.Implementations.Data.Enum;

namespace DependencyCrawler.Implementations.Models;

public class Reference : IReference
{
	public ReferenceType ReferenceType => ReferenceType.Unknown;
	public required IProject Using { get; set; }
	public IProject UsedBy { get; set; }
}