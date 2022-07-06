using System.Threading.Tasks;
using System.Windows;
using Core.Services.Interfaces.AppInfrastructure;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

internal sealed class CloseAppSc : ICloseAppSc
{
    private readonly ISyncDataOnServerSc _synchronizeDataOnServerSc;

    public CloseAppSc(ISyncDataOnServerSc synchronizeDataOnServerSc)
    {
        _synchronizeDataOnServerSc = synchronizeDataOnServerSc;
    }

    public async Task Close()
    {
        Application.Current.Shutdown();
        await _synchronizeDataOnServerSc?.SaveOnServer()!;
    }
}