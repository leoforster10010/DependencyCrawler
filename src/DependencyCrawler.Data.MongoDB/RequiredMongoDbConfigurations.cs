using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;

namespace DependencyCrawler.Data.MongoDB;

internal class RequiredMongoDbConfigurations : IRequiredConfigurations
{
	public IReadOnlyDictionary<ConfigurationKeys, ConfigurationTypes> Entries =>
		new Dictionary<ConfigurationKeys, ConfigurationTypes>
		{
			{ ConfigurationKeys.RootDirectory, ConfigurationTypes.Path },
			{ ConfigurationKeys.DllDirectory, ConfigurationTypes.Path }
		};
}