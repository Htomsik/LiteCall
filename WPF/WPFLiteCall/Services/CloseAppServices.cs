﻿using System.Windows;
using Core.Services.Interfaces.AppInfrastructure;

namespace LiteCall.Services;

internal sealed class CloseAppSc : ICloseAppSc
{
    private readonly ISyncDataOnServerSc _synchronizeDataOnServerSc;

    public CloseAppSc(ISyncDataOnServerSc synchronizeDataOnServerSc)
    {
        _synchronizeDataOnServerSc = synchronizeDataOnServerSc;
    }

    public async void Close()
    {
        Application.Current.Shutdown();
        await _synchronizeDataOnServerSc?.SaveOnServer()!;
    }
}