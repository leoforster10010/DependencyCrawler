using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Logging.Abstractions;

namespace DependencyCrawler.DataCore.Tests;

internal class ModuleTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		Assert.Multiple(() =>
		{
			Assert.That(newModule.Name is "test");
			Assert.That(newModule.References is { Count: 0 });
			Assert.That(newModule.Dependencies is { Count: 0 });
			Assert.That(newModule.DependencyLayer is 0);
			Assert.That(newModule.ReferenceLayer is 0);
			Assert.That(newModule.IsSubLevel);
			Assert.That(newModule.IsTopLevel);
		});
	}

	[Test]
	public void TestCreation()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var duplicateModule = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		Assert.That(duplicateModule, Is.SameAs(newModule));
	}

	[Test]
	public void TestAddDependency1()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var dependency = dataCore.GetOrCreateModule("dependency", ModuleType.Internal);

		module.AddDependency(dependency);

		Assert.Multiple(() =>
		{
			Assert.That(module.Dependencies is { Count: 1 });
			Assert.That(module.References is { Count: 0 });

			Assert.That(module.OutgoingReferences is { Count: 1 });
			Assert.That(module.IngoingReferences is { Count: 0 });

			Assert.That(dependency.References is { Count: 1 });
			Assert.That(dependency.Dependencies is { Count: 0 });

			Assert.That(dependency.IngoingReferences is { Count: 1 });
			Assert.That(dependency.OutgoingReferences is { Count: 0 });

			Assert.That(module.IsSubLevel);
			Assert.That(module.DependencyLayer is 1);
			Assert.That(module.ReferenceLayer is 0);

			Assert.That(dependency.IsTopLevel);
			Assert.That(dependency.DependencyLayer is 0);
			Assert.That(dependency.ReferenceLayer is 1);
		});
	}

	[Test]
	public void TestAddDependency2()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongDependency = dataCore2.GetOrCreateModule("dependency", ModuleType.Internal);

		module.AddDependency(wrongDependency);

		Assert.That(module.Dependencies is { Count: 0 });
	}

	[Test]
	public void TestAddDependency3()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var dependency = dataCore.GetOrCreateModule("dependency", ModuleType.Internal);

		module.AddDependency(dependency);

		var foundDependency = module.Dependencies["dependency"];
		Assert.That(foundDependency, Is.SameAs(dependency));
	}

	[Test]
	public void TestAddReference1()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var reference = dataCore.GetOrCreateModule("dependency", ModuleType.Internal);

		module.AddReference(reference);

		Assert.Multiple(() =>
		{
			Assert.That(module.Dependencies is { Count: 0 });
			Assert.That(module.References is { Count: 1 });

			Assert.That(module.OutgoingReferences is { Count: 0 });
			Assert.That(module.IngoingReferences is { Count: 1 });

			Assert.That(reference.References is { Count: 0 });
			Assert.That(reference.Dependencies is { Count: 1 });

			Assert.That(reference.IngoingReferences is { Count: 0 });
			Assert.That(reference.OutgoingReferences is { Count: 1 });

			Assert.That(module.IsTopLevel);
			Assert.That(module.DependencyLayer is 0);
			Assert.That(module.ReferenceLayer is 1);

			Assert.That(reference.IsSubLevel);
			Assert.That(reference.DependencyLayer is 1);
			Assert.That(reference.ReferenceLayer is 0);
		});
	}

	[Test]
	public void TestAddReference2()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongReference = dataCore2.GetOrCreateModule("reference", ModuleType.Internal);

		module.AddReference(wrongReference);

		Assert.That(module.References is { Count: 0 });
	}

	[Test]
	public void TestAddReference3()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var reference = dataCore.GetOrCreateModule("reference", ModuleType.Internal);

		module.AddReference(reference);

		var foundReference = module.References["reference"];
		Assert.That(foundReference, Is.SameAs(reference));
	}

	[Test]
	public void TestRemoveDependency()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;

		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var dependency = dataCore.GetOrCreateModule("dependency", ModuleType.Internal);

		module.AddDependency(dependency);
		module.RemoveDependency(dependency);

		Assert.Multiple(() =>
		{
			Assert.That(module.Dependencies is { Count: 0 });
			Assert.That(dependency.References is { Count: 0 });
		});
	}

	[Test]
	public void TestRemoveReference()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var reference = dataCore.GetOrCreateModule("reference", ModuleType.Internal);

		module.AddReference(reference);
		module.RemoveReference(reference);

		Assert.Multiple(() =>
		{
			Assert.That(module.References is { Count: 0 });
			Assert.That(reference.Dependencies is { Count: 0 });
		});
	}

	[Test]
	public void TestGetAllDependencies()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var dependency1 = dataCore.GetOrCreateModule("dependency1", ModuleType.Internal);
		var dependency2 = dataCore.GetOrCreateModule("dependency2", ModuleType.Internal);
		var dependency3 = dataCore.GetOrCreateModule("dependency3", ModuleType.Internal);

		module.AddDependency(dependency1);
		dependency1.AddDependency(dependency2);
		dependency2.AddDependency(dependency3);

		var allDependencies = module.GetAllDependencies();

		Assert.Multiple(() =>
		{
			Assert.That(allDependencies, Has.Count.EqualTo(3));
			Assert.That(allDependencies.ContainsKey("dependency1"));
			Assert.That(allDependencies.ContainsKey("dependency2"));
			Assert.That(allDependencies.ContainsKey("dependency3"));
			Assert.That(allDependencies["dependency1"], Is.SameAs(dependency1));
			Assert.That(allDependencies["dependency2"], Is.SameAs(dependency2));
			Assert.That(allDependencies["dependency3"], Is.SameAs(dependency3));
		});
	}

	[Test]
	public void TestGetAllReferences()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);
		var reference1 = dataCore.GetOrCreateModule("reference1", ModuleType.Internal);
		var reference2 = dataCore.GetOrCreateModule("reference2", ModuleType.Internal);
		var reference3 = dataCore.GetOrCreateModule("reference3", ModuleType.Internal);

		module.AddReference(reference1);
		reference1.AddReference(reference2);
		reference2.AddReference(reference3);

		var allReferences = module.GetAllReferences();

		Assert.Multiple(() =>
		{
			Assert.That(allReferences, Has.Count.EqualTo(3));
			Assert.That(allReferences.ContainsKey("reference1"));
			Assert.That(allReferences.ContainsKey("reference2"));
			Assert.That(allReferences.ContainsKey("reference3"));
			Assert.That(allReferences["reference1"], Is.SameAs(reference1));
			Assert.That(allReferences["reference2"], Is.SameAs(reference2));
			Assert.That(allReferences["reference3"], Is.SameAs(reference3));
		});
	}


	[Test]
	public void TestCollectionConsistency()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test", ModuleType.Internal);


		var dependencies = module.Dependencies;
		var dependenciesReadOnly = module.DependenciesReadOnly;
		var dependencyValues = module.DependencyValues;
		var outgoingReferences = module.OutgoingReferences;

		var references = module.References;
		var referencesReadOnly = module.ReferencesReadOnly;
		var referenceValues = module.ReferenceValues;
		var ingoingReferences = module.IngoingReferences;

		var dependency1 = dataCore.GetOrCreateModule("dependency1", ModuleType.Internal);
		module.AddDependency(dependency1);
		var reference1 = dataCore.GetOrCreateModule("reference1", ModuleType.Internal);
		module.AddReference(reference1);


		Assert.That(dependencies, Has.Count.EqualTo(1));
		Assert.That(dependenciesReadOnly, Has.Count.EqualTo(1));
		Assert.That(dependencyValues, Has.Count.EqualTo(1));
		Assert.That(outgoingReferences, Has.Count.EqualTo(1));
		Assert.That(references, Has.Count.EqualTo(1));
		Assert.That(referencesReadOnly, Has.Count.EqualTo(1));
		Assert.That(referenceValues, Has.Count.EqualTo(1));
		Assert.That(ingoingReferences, Has.Count.EqualTo(1));
	}
}