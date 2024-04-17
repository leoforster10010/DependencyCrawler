namespace DependencyCrawler.DataCore;

internal class ModuleRepository : IModuleRepository
{
	private readonly IDependencyCrawlerDataAccess _dependencyCrawlerDataAccess;

	public ModuleRepository(IDependencyCrawlerDataAccess dependencyCrawlerDataAccess)
	{
		_dependencyCrawlerDataAccess = dependencyCrawlerDataAccess;
	}

	public void Update(IValueModule valueModule)
	{
		if (!_dependencyCrawlerDataAccess.Core.Modules.TryGetValue(valueModule.NameValue, out var module))
		{
			Add(valueModule);
			return;
		}


		foreach (var dependencyOfValue in valueModule.DependencyOfValue.Where(dependencyOfValue => !module.DependencyOf.ContainsKey(dependencyOfValue.Key)))
		{
			module.AddDependencyOf(dependencyOfValue.Key);
		}

		foreach (var dependencyOfValue in module.DependencyOf)
		{
			if (valueModule.DependencyOfValue.ContainsKey(dependencyOfValue.Key))
			{
				continue;
			}

			if (_dependencyCrawlerDataAccess.Core.Modules.TryGetValue(dependencyOfValue.Key, out var dependencyOf))
			{
				dependencyOf.DependingOn.TryRemove(dependencyOfValue);
			}

			module.DependencyOf.TryRemove(dependencyOfValue);
		}


		foreach (var dependingOnValue in valueModule.DependingOnValue.Where(dependingOnValue => !module.DependingOn.ContainsKey(dependingOnValue.Key)))
		{
			module.AddDependingOn(dependingOnValue.Key);
		}

		foreach (var dependingOnValue in module.DependingOn)
		{
			if (valueModule.DependencyOfValue.ContainsKey(dependingOnValue.Key))
			{
				continue;
			}

			if (_dependencyCrawlerDataAccess.Core.Modules.TryGetValue(dependingOnValue.Key, out var dependingOn))
			{
				dependingOn.DependencyOf.TryRemove(dependingOnValue);
			}

			module.DependingOn.TryRemove(dependingOnValue);
		}
	}

	public void Add(IValueModule valueModule)
	{
		if (_dependencyCrawlerDataAccess.Core.Modules.ContainsKey(valueModule.NameValue))
		{
			return;
		}

		var module = new Module
		{
			Name = valueModule.NameValue,
			DependencyCrawlerCore = _dependencyCrawlerDataAccess.Core
		};

		foreach (var dependencyOfValue in valueModule.DependencyOfValue)
		{
			module.AddDependencyOf(dependencyOfValue.Key);
		}

		foreach (var dependingOnValue in valueModule.DependingOnValue)
		{
			module.AddDependingOn(dependingOnValue.Key);
		}

		_dependencyCrawlerDataAccess.Core.Modules.TryAdd(module.Name, module);
	}
}