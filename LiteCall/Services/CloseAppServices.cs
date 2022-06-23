using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

internal sealed class CloseAppServices : ICloseAppServices
{
    private readonly ISynhronyzeDataOnServerServices _synchronizeDataOnServerServices;

    public CloseAppServices(ISynhronyzeDataOnServerServices synchronizeDataOnServerServices)
    {
        _synchronizeDataOnServerServices = synchronizeDataOnServerServices;
    }

    public async Task Close()
    {
        Application.Current.Shutdown();
        await _synchronizeDataOnServerServices?.SaveOnServer()!;
    }
}