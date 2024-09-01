namespace DependencyCrawler.DataCore.Tests;

internal class ModuleTests
{
	[Test]
	public void TestState()
	{
		var dataCoreProvider = new DataCoreProvider();
		var dataCore = dataCoreProvider.CreateDataCore();
		var newModule = dataCore.CreateModule("test");

		Assert.IsTrue(newModule.Name is "test");
		Assert.IsTrue(newModule.References.Count == 0);
		Assert.IsTrue(newModule.Dependencies.Count == 0);
		Assert.IsTrue(newModule.DependencyLayer == 0);
		Assert.IsTrue(newModule.ReferenceLayer == 0);
		Assert.IsTrue(newModule.IsSubLevel);
		Assert.IsTrue(newModule.IsTopLevel);
	}
}