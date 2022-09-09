using System.Windows;
using Core.Services.Interfaces.AppInfrastructure;

namespace LiteCall.Services;

internal sealed class CloseAppSc : ICloseAppSc
{
    // private readonly ISyncDataOnServerSc _synchronizeDataOnServerSc;
    //
    // public CloseAppSc(ISyncDataOnServerSc synchronizeDataOnServerSc)
    // {
    //     _synchronizeDataOnServerSc = synchronizeDataOnServerSc;
    // }

    public CloseAppSc(){}
    
    public async void Close()
    {
        Application.Current.Shutdown();
        
        // Temporary removed (Maybe not temporary if u want remove main server)
        // await _synchronizeDataOnServerSc?.SaveOnServer()!;
    }
}