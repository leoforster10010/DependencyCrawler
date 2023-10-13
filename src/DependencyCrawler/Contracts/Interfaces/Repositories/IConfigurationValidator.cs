using DependencyCrawler.Data.Contracts.Enum;

namespace DependencyCrawler.Contracts.Interfaces.Repositories;

public interface IConfigurationValidator
{
	bool IsConfigurationValid();
	IReadOnlyList<ConfigurationKeys> GetInvalidConfigurations();
}