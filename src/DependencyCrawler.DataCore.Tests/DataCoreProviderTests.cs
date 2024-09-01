namespace DependencyCrawler.DataCore.Tests;

internal class DataCoreProviderTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		Assert.IsTrue(dataCoreProvider.DataCores.Count == 1);
		Assert.IsTrue(dataCoreProvider.ActiveCore is not null);
	}

	[Test]
	public void TestMethodsDataCoreCreation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		Assert.IsTrue(dataCoreProvider.DataCores.Count == 2);
		Assert.IsFalse(newDataCore.IsActive);
		Assert.AreSame(newDataCore.DataCoreProvider, dataCoreProvider);
	}


	[Test]
	public void TestMethodsDataCoreActivation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		newDataCore.Activate();

		Assert.AreSame(dataCoreProvider.ActiveCore, newDataCore);
		Assert.IsTrue(newDataCore.IsActive);
	}


	[Test]
	public void TestMethodsDataCoreDeletion()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		newDataCore.Delete();
		Assert.IsTrue(dataCoreProvider.DataCores.Count == 1);

		dataCoreProvider.ActiveCore.Delete();
		Assert.IsTrue(dataCoreProvider.DataCores.Count == 1);
	}
}