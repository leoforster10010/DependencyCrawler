using Microsoft.Extensions.Logging.Abstractions;

namespace DependencyCrawler.DataCore.Tests;

internal class ModuleTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.GetOrCreateModule("test");

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
		var newModule = dataCore.GetOrCreateModule("test");
		var duplicateModule = dataCore.GetOrCreateModule("test");

		Assert.That(duplicateModule, Is.SameAs(newModule));
	}

	[Test]
	public void TestAddDependency1()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test");
		var dependency = dataCore.GetOrCreateModule("dependency");

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
		var module = dataCore.GetOrCreateModule("test");

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongDependency = dataCore2.GetOrCreateModule("dependency");

		module.AddDependency(wrongDependency);

		Assert.That(module.Dependencies is { Count: 0 });
	}

	[Test]
	public void TestAddDependency3()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test");
		var dependency = dataCore.GetOrCreateModule("dependency");

		module.AddDependency(dependency);

		var foundDependency = module.Dependencies["dependency"];
		Assert.That(foundDependency, Is.SameAs(dependency));
	}

	[Test]
	public void TestAddReference1()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test");
		var reference = dataCore.GetOrCreateModule("dependency");

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
		var module = dataCore.GetOrCreateModule("test");

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongReference = dataCore2.GetOrCreateModule("reference");

		module.AddReference(wrongReference);

		Assert.That(module.References is { Count: 0 });
	}

	[Test]
	public void TestAddReference3()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.GetOrCreateModule("test");
		var reference = dataCore.GetOrCreateModule("reference");

		module.AddReference(reference);

		var foundReference = module.References["reference"];
		Assert.That(foundReference, Is.SameAs(reference));
	}

	[Test]
	public void TestRemoveDependency()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;

		var module = dataCore.GetOrCreateModule("test");
		var dependency = dataCore.GetOrCreateModule("dependency");

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
		var module = dataCore.GetOrCreateModule("test");
		var reference = dataCore.GetOrCreateModule("reference");

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
		var module = dataCore.GetOrCreateModule("test");
		var dependency1 = dataCore.GetOrCreateModule("dependency1");
		var dependency2 = dataCore.GetOrCreateModule("dependency2");
		var dependency3 = dataCore.GetOrCreateModule("dependency3");

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
		var module = dataCore.GetOrCreateModule("test");
		var reference1 = dataCore.GetOrCreateModule("reference1");
		var reference2 = dataCore.GetOrCreateModule("reference2");
		var reference3 = dataCore.GetOrCreateModule("reference3");

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
}