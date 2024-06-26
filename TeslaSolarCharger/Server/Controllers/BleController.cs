﻿using Microsoft.AspNetCore.Mvc;
using TeslaSolarCharger.Server.Services.Contracts;
using TeslaSolarCharger.SharedBackend.Abstracts;

namespace TeslaSolarCharger.Server.Controllers;

public class BleController (IBleService bleService) : ApiBaseController
{
    [HttpGet]
    public Task<string> PairKey(string vin) => bleService.PairKey(vin);

    [HttpGet]
    public Task StartCharging(string vin) => bleService.StartCharging(vin);

    [HttpGet]
    public Task StopCharging(string vin) => bleService.StopCharging(vin);

    [HttpGet]
    public Task SetAmp(string vin, int amps) => bleService.SetAmp(vin, amps);
}
