using DependencyCrawler.DataCore.ValueAccess;

namespace DependencyCrawler.DataCore.DataAccess;

internal class ModuleRepository(IDataAccess dataAccess) : IModuleRepository
{
	public void Update(IValueModule valueModule)
	{
		if (!dataAccess.Core.Modules.TryGetValue(valueModule.Name, out var module))
		{
			Add(valueModule);
			return;
		}

		foreach (var referenceValue in valueModule.ReferencesValue.Where(referenceValue => !module.References.ContainsKey(referenceValue)))
		{
			module.AddReference(referenceValue);
		}

		foreach (var referenceValue in module.References)
		{
			if (valueModule.ReferencesValue.Contains(referenceValue.Key))
			{
				continue;
			}

			if (dataAccess.Core.Modules.TryGetValue(referenceValue.Key, out var reference))
			{
				reference.Dependencies.TryRemove(referenceValue);
			}

			module.References.TryRemove(referenceValue);
		}


		foreach (var dependencyValue in valueModule.DependenciesValue.Where(dependencyValue => !module.Dependencies.ContainsKey(dependencyValue)))
		{
			module.AddDependency(dependencyValue);
		}

		foreach (var dependencyValue in module.Dependencies)
		{
			if (valueModule.ReferencesValue.Contains(dependencyValue.Key))
			{
				continue;
			}

			if (dataAccess.Core.Modules.TryGetValue(dependencyValue.Key, out var dependency))
			{
				dependency.References.TryRemove(dependencyValue);
			}

			module.Dependencies.TryRemove(dependencyValue);
		}
	}

	public void Add(IValueModule valueModule)
	{
		if (dataAccess.Core.Modules.ContainsKey(valueModule.Name))
		{
			return;
		}

		var module = new Module
		{
			Name = valueModule.Name,
			DataCore = dataAccess.Core,
			Type = valueModule.Type
		};

		foreach (var referenceValue in valueModule.ReferencesValue)
		{
			module.AddReference(referenceValue);
		}

		foreach (var dependencyValue in valueModule.DependenciesValue)
		{
			module.AddDependency(dependencyValue);
		}

		dataAccess.Core.Modules.TryAdd(module.Name, module);
	}
}