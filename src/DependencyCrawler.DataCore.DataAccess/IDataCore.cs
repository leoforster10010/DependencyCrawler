using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore.DataAccess;

public interface IDataCore
{
	ConcurrentDictionary<string, Module> Modules { get; }
	Guid Id { get; }
}