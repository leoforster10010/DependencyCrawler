using DependencyCrawler.DataCore.ValueAccess;
using Microsoft.Extensions.Logging.Abstractions;

namespace DependencyCrawler.DataCore.Tests;

internal class DataCoreTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;

		Assert.Multiple(() =>
		{
			Assert.That(dataCore.Modules.Count is 0);
			Assert.That(dataCore.Entities.Count is 0);
		});
	}

	[Test]
	public void TestMethodsModuleCreation()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		Assert.Multiple(() =>
		{
			Assert.That(newModule.DataCoreProvider, Is.SameAs(dataCoreProvider));
			Assert.That(newModule.DataCore, Is.SameAs(dataCore));
			Assert.That(dataCore.Modules.Count is 1);
			Assert.That(dataCore.Entities.Count is 1);
		});
	}

	[Test]
	public void TestMethodsModuleDeletion()
	{
		var dataCoreProvider = new DataCoreProvider(new NullLogger<DataCoreProvider>());
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.GetOrCreateModule("test", ModuleType.Internal);

		newModule.Delete();
		Assert.Multiple(() =>
		{
			Assert.That(dataCore.Modules.Count is 0);
			Assert.That(dataCore.Entities.Count is 0);
		});
	}
}