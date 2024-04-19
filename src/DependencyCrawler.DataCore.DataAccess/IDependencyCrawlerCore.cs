﻿using System.Collections.Concurrent;

namespace DependencyCrawler.DataCore.DataAccess;

public interface IDependencyCrawlerCore
{
	ConcurrentDictionary<string, Module> Modules { get; }
	Guid Id { get; }
}