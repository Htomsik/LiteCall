using System.Windows.Input;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using Core.VMD.AdditionalVmds.SettingsVmds.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.AdditionalVmds.SettingsVmds;

/// <summary>
///     Saved servers vmd in Settings vmd
/// </summary>
public class SavedServersSettingsVmd : BaseSettingsVmd
{
    #region Stores

    private SavedServersStore _savedServersStore;

    #endregion

    #region Services

    private IHttpDataSc _httpDataSc;

    private IStatusSc _statusSc;

    #endregion

    #region Properties and Fields

    /// <summary>
    ///     Saved servers in current main Account
    /// </summary>
    public AppSavedServers? SavedServers => _savedServersStore.CurrentValue;
    
    /// <summary>
    ///     Ip of Authorized Api server
    /// </summary>
    [Reactive]
    public string? NewServerApiIp { get; set; }
    
    /// <summary>
    ///     Account login of added server
    /// </summary>
    [Reactive]
    public string? NewSeverLogin { get; set; }
    
    #endregion

    
    public SavedServersSettingsVmd(
        AppSettingsStore appSettingsStore,
        SavedServersStore savedServersStore,
        IHttpDataSc httpDataSc,
        IStatusSc statusService) : base(appSettingsStore)
    {
        #region Stores and Services Initializing

        _savedServersStore = savedServersStore;

        _httpDataSc = httpDataSc;

        _statusSc = statusService;

        #endregion
        
        #region Properties and Fields Initializing

        

        #endregion

        #region Subscription

        _savedServersStore.CurrentValueChangedNotifier += () => this.RaisePropertyChanged(nameof(SavedServers));

        #endregion
        
        #region Commnads Initializing

        AddNewServerCommand = ReactiveCommand.CreateFromTask(OnAddNewServerExecuted, CanAddNewServerExecute());

        #endregion
    }

    #region Commands

    #region AddNewServerCommand : Add new server commad 

    public  ICommand AddNewServerCommand { get;  }
    
    private IObservable<bool> CanAddNewServerExecute() => this.WhenAnyValue(x=>
            x.NewServerApiIp,
        x=>x.NewSeverLogin,
        (newServerApiIp, newSeverLogin) =>
            !string.IsNullOrEmpty(newServerApiIp) && !string.IsNullOrEmpty(newSeverLogin));
    

    private async Task OnAddNewServerExecuted()
    {
        var newSavedSeverAccount = new ServerAccount
        {
            Account = new Account { Login = NewSeverLogin }
        };


        var serverStatus = await Task.Run(() => _httpDataSc.CheckServerStatus(NewServerApiIp));

        if (serverStatus)
        {
            var newServer = await _httpDataSc.ApiServerGetInfo(NewServerApiIp);

            if (newServer == null) return;

            newServer.ApiIp = NewServerApiIp;

            newSavedSeverAccount.SavedServer = newServer;

            try
            {
                _savedServersStore!.Add(newSavedSeverAccount);
            }
            catch (Exception)
            {
                _statusSc.ChangeStatus("Server already exist");
            }
        
                
        }
    }

    #endregion

    #endregion
}