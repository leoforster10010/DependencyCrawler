using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

internal class ModuleRepository(IDependencyCrawlerDataAccess dependencyCrawlerDataAccess) : IModuleRepository
{
	public void Update(IValueModule valueModule)
	{
		if (!dependencyCrawlerDataAccess.Core.Modules.TryGetValue(valueModule.NameValue, out var module))
		{
			Add(valueModule);
			return;
		}


		foreach (var dependencyOfValue in valueModule.DependencyOfValue.Where(dependencyOfValue => !module.DependencyOf.ContainsKey(dependencyOfValue)))
		{
			module.AddDependencyOf(dependencyOfValue);
		}

		foreach (var dependencyOfValue in module.DependencyOf)
		{
			if (valueModule.DependencyOfValue.Contains(dependencyOfValue.Key))
			{
				continue;
			}

			if (dependencyCrawlerDataAccess.Core.Modules.TryGetValue(dependencyOfValue.Key, out var dependencyOf))
			{
				dependencyOf.DependingOn.TryRemove(dependencyOfValue);
			}

			module.DependencyOf.TryRemove(dependencyOfValue);
		}


		foreach (var dependingOnValue in valueModule.DependingOnValue.Where(dependingOnValue => !module.DependingOn.ContainsKey(dependingOnValue)))
		{
			module.AddDependingOn(dependingOnValue);
		}

		foreach (var dependingOnValue in module.DependingOn)
		{
			if (valueModule.DependencyOfValue.Contains(dependingOnValue.Key))
			{
				continue;
			}

			if (dependencyCrawlerDataAccess.Core.Modules.TryGetValue(dependingOnValue.Key, out var dependingOn))
			{
				dependingOn.DependencyOf.TryRemove(dependingOnValue);
			}

			module.DependingOn.TryRemove(dependingOnValue);
		}
	}

	public void Add(IValueModule valueModule)
	{
		if (dependencyCrawlerDataAccess.Core.Modules.ContainsKey(valueModule.NameValue))
		{
			return;
		}

		var module = new Module
		{
			Name = valueModule.NameValue,
			DependencyCrawlerCore = dependencyCrawlerDataAccess.Core
		};

		foreach (var dependencyOfValue in valueModule.DependencyOfValue)
		{
			module.AddDependencyOf(dependencyOfValue);
		}

		foreach (var dependingOnValue in valueModule.DependingOnValue)
		{
			module.AddDependingOn(dependingOnValue);
		}

		dependencyCrawlerDataAccess.Core.Modules.TryAdd(module.Name, module);
	}
}