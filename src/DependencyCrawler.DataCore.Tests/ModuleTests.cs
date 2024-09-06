namespace DependencyCrawler.DataCore.Tests;

internal class ModuleTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.CreateModule("test");

		Assert.IsTrue(newModule.Name is "test");
		Assert.IsTrue(newModule.References is { Count: 0 });
		Assert.IsTrue(newModule.Dependencies is { Count: 0 });
		Assert.IsTrue(newModule.DependencyLayer is 0);
		Assert.IsTrue(newModule.ReferenceLayer is 0);
		Assert.IsTrue(newModule.IsSubLevel);
		Assert.IsTrue(newModule.IsTopLevel);
	}

	[Test]
	public void TestCreation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.CreateModule("test");
		var duplikaModule = dataCore.CreateModule("test");

		Assert.AreSame(newModule, duplikaModule);
	}

	[Test]
	public void TestAddDependency1()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var dependency = dataCore.CreateModule("dependency");

		module.AddDependency(dependency);

		Assert.IsTrue(module.Dependencies is { Count: 1 });
		Assert.IsTrue(module.References is { Count: 0 });

		Assert.IsTrue(module.OutgoingReferences is { Count: 1 });
		Assert.IsTrue(module.IngoingReferences is { Count: 0 });

		Assert.IsTrue(dependency.References is { Count: 1 });
		Assert.IsTrue(dependency.Dependencies is { Count: 0 });

		Assert.IsTrue(dependency.IngoingReferences is { Count: 1 });
		Assert.IsTrue(dependency.OutgoingReferences is { Count: 0 });

		Assert.IsTrue(module.IsSubLevel);
		Assert.IsTrue(module.DependencyLayer is 1);
		Assert.IsTrue(module.ReferenceLayer is 0);

		Assert.IsTrue(dependency.IsTopLevel);
		Assert.IsTrue(dependency.DependencyLayer is 0);
		Assert.IsTrue(dependency.ReferenceLayer is 1);
	}

	[Test]
	public void TestAddDependency2()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongDependency = dataCore2.CreateModule("dependency");

		module.AddDependency(wrongDependency);

		Assert.IsTrue(module.Dependencies is { Count: 0 });
	}

	[Test]
	public void TestAddDependency3()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var dependency = dataCore.CreateModule("dependency");

		module.AddDependency(dependency);

		var foundDependency = module.Dependencies["dependency"];
		Assert.AreSame(dependency, foundDependency);
	}

	[Test]
	public void TestAddReference1()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var reference = dataCore.CreateModule("dependency");

		module.AddReference(reference);

		Assert.IsTrue(module.Dependencies is { Count: 0 });
		Assert.IsTrue(module.References is { Count: 1 });

		Assert.IsTrue(module.OutgoingReferences is { Count: 0 });
		Assert.IsTrue(module.IngoingReferences is { Count: 1 });

		Assert.IsTrue(reference.References is { Count: 0 });
		Assert.IsTrue(reference.Dependencies is { Count: 1 });

		Assert.IsTrue(reference.IngoingReferences is { Count: 0 });
		Assert.IsTrue(reference.OutgoingReferences is { Count: 1 });

		Assert.IsTrue(module.IsTopLevel);
		Assert.IsTrue(module.DependencyLayer is 0);
		Assert.IsTrue(module.ReferenceLayer is 1);

		Assert.IsTrue(reference.IsSubLevel);
		Assert.IsTrue(reference.DependencyLayer is 1);
		Assert.IsTrue(reference.ReferenceLayer is 0);
	}

	[Test]
	public void TestAddReference2()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");

		var dataCore2 = dataCoreProvider.CreateDataCore();
		var wrongReference = dataCore2.CreateModule("reference");

		module.AddReference(wrongReference);

		Assert.IsTrue(module.References is { Count: 0 });
	}

	[Test]
	public void TestAddReference3()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var reference = dataCore.CreateModule("reference");

		module.AddReference(reference);

		var foundReference = module.References["reference"];
		Assert.AreSame(reference, foundReference);
	}

	[Test]
	public void TestRemoveDependency()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var dependency = dataCore.CreateModule("dependency");

		module.AddDependency(dependency);
		module.RemoveDependency(dependency);

		Assert.IsTrue(module.Dependencies is { Count: 0 });
		Assert.IsTrue(dependency.References is { Count: 0 });
	}

	[Test]
	public void TestRemoveReference()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var module = dataCore.CreateModule("test");
		var reference = dataCore.CreateModule("reference");

		module.AddReference(reference);
		module.RemoveReference(reference);

		Assert.IsTrue(module.References is { Count: 0 });
		Assert.IsTrue(reference.Dependencies is { Count: 0 });
	}
}