using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Data.Contracts.Interfaces;

public interface IRequiredConfigurations
{
	IReadOnlyDictionary<ConfigurationKeys, ConfigurationTypes> Entries { get; }
}