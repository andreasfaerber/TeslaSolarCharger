﻿using TeslaSolarCharger.Server.Contracts;
using TeslaSolarCharger.Services.Services.Mqtt.Contracts;

namespace TeslaSolarCharger.Server.Services;

public class MqttConnectionService(
    ILogger<MqttConnectionService> logger,
    ITeslaMateMqttService teslaMateMqttService,
    IMqttClientReconnectionService mqttClientReconnectionService)
    : IMqttConnectionService
{
    public async Task ReconnectMqttServices()
    {
        logger.LogTrace("{method}()", nameof(ReconnectMqttServices));
        try
        {
            await teslaMateMqttService.ConnectClientIfNotConnected().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while connecting TeslaMateMqttService");
        }

        try
        {
            await mqttClientReconnectionService.ReconnectMqttClients().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while reconnecting MqttClients");
        }
    }
}
