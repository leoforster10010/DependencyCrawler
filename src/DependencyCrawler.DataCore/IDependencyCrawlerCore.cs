using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore;

internal interface IDependencyCrawlerCore
{
	ConcurrentDictionary<string, Module> Modules { get; }
	Guid Id { get; }
	IDependencyCrawlerDataAccess DependencyCrawlerDataAccess { get; init; }
	bool IsActive { get; }
}