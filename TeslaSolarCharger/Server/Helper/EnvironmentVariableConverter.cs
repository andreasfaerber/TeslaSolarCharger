﻿using TeslaSolarCharger.Server.Contracts;
using TeslaSolarCharger.Shared.Dtos.BaseConfiguration;

namespace TeslaSolarCharger.Server.Helper;

public class EnvironmentVariableConverter : IEnvironmentVariableConverter
{
    private readonly ILogger<EnvironmentVariableConverter> _logger;
    private readonly IConfiguration _configuration;
    private readonly IBaseConfigurationService _baseConfigurationService;

    public EnvironmentVariableConverter(ILogger<EnvironmentVariableConverter> logger, IConfiguration configuration,
        IBaseConfigurationService baseConfigurationService)
    {
        _logger = logger;
        _configuration = configuration;
        _baseConfigurationService = baseConfigurationService;
    }

    public async Task ConvertAllValues()
    {
        _logger.LogTrace("{method}()", nameof(ConvertAllValues));
        if (await _baseConfigurationService.IsBaseConfigurationJsonRelevant())
        {
            _logger.LogInformation("Do not convert environment variables to json file as json file has been edited.");
            return;
        }
        var dtoBaseConfiguration = new DtoBaseConfiguration()
        {
            CurrentPowerToGridUrl = _configuration.GetValue<string>("CurrentPowerToGridUrl"),
            CurrentInverterPowerUrl = _configuration.GetValue<string?>("CurrentInverterPowerUrl"),
            TeslaMateApiBaseUrl = _configuration.GetValue<string>("TeslaMateApiBaseUrl"),
            UpdateIntervalSeconds = _configuration.GetValue<int>("UpdateIntervalSeconds"),
            PvValueUpdateIntervalSeconds = _configuration.GetValue<int>("PvValueUpdateIntervalSeconds"),
            CarPriorities = _configuration.GetValue<string>("CarPriorities"),
            GeoFence = _configuration.GetValue<string>("GeoFence"),
            MinutesUntilSwitchOn = _configuration.GetValue<int>("MinutesUntilSwitchOn"),
            MinutesUntilSwitchOff = _configuration.GetValue<int>("MinutesUntilSwitchOff"),
            PowerBuffer = _configuration.GetValue<int>("PowerBuffer"),
            CurrentPowerToGridJsonPattern = _configuration.GetValue<string?>("CurrentPowerToGridJsonPattern"),
            CurrentPowerToGridInvertValue = _configuration.GetValue<bool?>("CurrentPowerToGridInvertValue") == true,
            CurrentInverterPowerJsonPattern = _configuration.GetValue<string?>("CurrentInverterPowerJsonPattern"),
            TelegramBotKey = _configuration.GetValue<string?>("TelegramBotKey"),
            TelegramChannelId = _configuration.GetValue<string?>("TelegramChannelId"),
            TeslaMateDbServer = _configuration.GetValue<string>("TeslaMateDbServer"),
            TeslaMateDbPort = _configuration.GetValue<int>("TeslaMateDbPort"),
            TeslaMateDbDatabaseName = _configuration.GetValue<string>("TeslaMateDbDatabaseName"),
            TeslaMateDbUser = _configuration.GetValue<string>("TeslaMateDbUser"),
            TeslaMateDbPassword = _configuration.GetValue<string>("TeslaMateDbPassword"),
            MqqtClientId = _configuration.GetValue<string>("MqqtClientId"),
            MosquitoServer = _configuration.GetValue<string>("MosquitoServer"),
            CurrentPowerToGridXmlPattern = _configuration.GetValue<string?>("CurrentPowerToGridXmlPattern"),
            CurrentPowerToGridXmlAttributeHeaderName = _configuration.GetValue<string?>("CurrentPowerToGridXmlAttributeHeaderName"),
            CurrentPowerToGridXmlAttributeHeaderValue = _configuration.GetValue<string?>("CurrentPowerToGridXmlAttributeHeaderValue"),
            CurrentPowerToGridXmlAttributeValueName = _configuration.GetValue<string?>("CurrentPowerToGridXmlAttributeValueName"),
            CurrentInverterPowerXmlPattern = _configuration.GetValue<string?>("CurrentInverterPowerXmlPattern"),
            CurrentInverterPowerXmlAttributeHeaderName = _configuration.GetValue<string?>("CurrentInverterPowerAttributeHeaderName"),
            CurrentInverterPowerXmlAttributeHeaderValue = _configuration.GetValue<string?>("CurrentInverterPowerAttributeHeaderValue"),
            CurrentInverterPowerXmlAttributeValueName = _configuration.GetValue<string?>("CurrentInverterPowerAttributeValueName"),
        };

        await _baseConfigurationService.SaveBaseConfiguration(dtoBaseConfiguration);
    }
}