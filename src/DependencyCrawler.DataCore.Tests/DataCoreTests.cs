namespace DependencyCrawler.DataCore.Tests;

internal class DataCoreTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;

		Assert.IsTrue(dataCore.Modules.Count is 0);
		Assert.IsTrue(dataCore.Entities.Count is 0);
	}

	[Test]
	public void TestMethodsModuleCreation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.CreateModule("test");

		Assert.AreSame(dataCoreProvider, newModule.DataCoreProvider);
		Assert.AreSame(dataCore, newModule.DataCore);
		Assert.IsTrue(dataCore.Modules.Count is 1);
		Assert.IsTrue(dataCore.Entities.Count is 1);
	}

	[Test]
	public void TestMethodsModuleDeletion()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.ActiveCore;
		var newModule = dataCore.CreateModule("test");

		newModule.Delete();
		Assert.IsTrue(dataCore.Modules.Count is 0);
		Assert.IsTrue(dataCore.Entities.Count is 0);
	}
}