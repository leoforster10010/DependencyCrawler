namespace DependencyCrawler.DataCore.Tests;

internal class ModuleTests
{
    [Test]
    public void TestState()
    {
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
        var dataCore = dataCoreProvider.ActiveCore;
        var newModule = dataCore.GetOrCreateModule("test");
        var duplicateModule = dataCore.GetOrCreateModule("test");

        Assert.That(duplicateModule, Is.SameAs(newModule));
    }

    [Test]
    public void TestAddDependency1()
    {
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
        var dataCoreProvider = new DataCoreProvider();
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
}