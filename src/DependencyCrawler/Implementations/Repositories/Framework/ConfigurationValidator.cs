using DependencyCrawler.Contracts.Interfaces.Repositories;
using DependencyCrawler.Data.Contracts.Enum;
using DependencyCrawler.Data.Contracts.Interfaces;
using Microsoft.Extensions.Configuration;

namespace DependencyCrawler.Implementations.Repositories.Framework;

internal class ConfigurationValidator : IConfigurationValidator
{
	private readonly IConfiguration _configuration;

	private readonly IDictionary<ConfigurationKeys, ConfigurationTypes> _requiredConfigurations =
		new Dictionary<ConfigurationKeys, ConfigurationTypes>();

	public ConfigurationValidator(IConfiguration configuration,
		IEnumerable<IRequiredConfigurations> requiredConfigurations)
	{
		_configuration = configuration;

		foreach (var configurationEntry in requiredConfigurations.SelectMany(x => x.Entries))
		{
			_requiredConfigurations.TryAdd(configurationEntry.Key, configurationEntry.Value);
		}
	}

	public bool IsConfigurationValid()
	{
		return _requiredConfigurations.All(IsConfigurationEntryValid);
	}

	public IReadOnlyList<ConfigurationKeys> GetInvalidConfigurations()
	{
		var invalidEntries = new List<ConfigurationKeys>();

		foreach (var requiredConfiguration in _requiredConfigurations)
		{
			if (!IsConfigurationEntryValid(requiredConfiguration))
			{
				invalidEntries.Add(requiredConfiguration.Key);
			}
		}

		return invalidEntries;
	}

	private bool IsConfigurationEntryValid(KeyValuePair<ConfigurationKeys, ConfigurationTypes> configurationEntry)
	{
		switch (configurationEntry.Value)
		{
			case ConfigurationTypes.Path:
				return IsPathValid(configurationEntry.Key);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private bool IsPathValid(ConfigurationKeys pathEntryKey)
	{
		var path = _configuration[pathEntryKey.ToString()];

		return Directory.Exists(path);
	}
}