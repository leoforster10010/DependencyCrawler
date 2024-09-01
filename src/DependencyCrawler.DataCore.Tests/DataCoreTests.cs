namespace DependencyCrawler.DataCore.Tests;

internal class DataCoreTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.CreateDataCore();

		Assert.IsTrue(dataCore.Modules.Count == 0);
		Assert.IsTrue(dataCore.Entities.Count == 0);
	}

	[Test]
	public void TestMethodsModuleCreation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.CreateDataCore();
		var newModule = dataCore.CreateModule("test");

		Assert.AreSame(dataCoreProvider, newModule.DataCoreProvider);
		Assert.AreSame(dataCore, newModule.DataCore);
		Assert.IsTrue(dataCore.Modules.Count == 1);
		Assert.IsTrue(dataCore.Entities.Count == 1);
	}

	[Test]
	public void TestMethodsModuleDeletion()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.CreateDataCore();
		var newModule = dataCore.CreateModule("test");

		newModule.Delete();
		Assert.IsTrue(dataCore.Modules.Count == 0);
		Assert.IsTrue(dataCore.Entities.Count == 0);
	}
}