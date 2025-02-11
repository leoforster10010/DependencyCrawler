namespace DependencyCrawler.DataCore.Tests;

internal class DataCoreProviderTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		Assert.Multiple(() => { Assert.That(dataCoreProvider.DataCores is { Count: 1 }); });
	}

	[Test]
	public void TestMethodsDataCoreCreation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		Assert.Multiple(() =>
		{
			Assert.That(dataCoreProvider.DataCores is { Count: 2 });
			Assert.That(!newDataCore.IsActive);
			Assert.That(dataCoreProvider, Is.SameAs(newDataCore.DataCoreProvider));
		});
	}


	[Test]
	public void TestMethodsDataCoreActivation()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		newDataCore.Activate();

		Assert.Multiple(() =>
		{
			Assert.That(newDataCore.IsActive);
			Assert.That(newDataCore, Is.SameAs(dataCoreProvider.ActiveCore));
		});
	}


	[Test]
	public void TestMethodsDataCoreDeletion()
	{
		var dataCoreProvider = new DataCoreProvider();
		var newDataCore = dataCoreProvider.CreateDataCore();

		newDataCore.Delete();
		Assert.That(dataCoreProvider.DataCores is { Count: 1 });

		dataCoreProvider.ActiveCore.Delete();
		Assert.That(dataCoreProvider.DataCores is { Count: 1 });
	}
}