using System.Collections.ObjectModel;
using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Main.HubVmds;

/// <summary>
///     SavedServersManageVmd
/// </summary>
public class SavedServersVmd : BaseVmd
{
    #region Stores

    private readonly SavedServersStore _savedServersStore;
    
    private readonly CurrentServerStore _currentServerStore;

    #endregion

    #region Services

    private readonly IHttpDataSc _httpDataSc;

    private readonly INavigationServices _serverPageNavigationServices;

    private readonly IAuthorizationSc _authorizationApiServices;

    #endregion

    #region Properties 

    /// <summary>
    ///     Saved servers in current main account
    /// </summary>
    public ObservableCollection<ServerAccount>? SavedServes { get; private set; }
    
    /// <summary>
    ///     Current selected saved server account
    /// </summary>
    [Reactive]
    public ServerAccount SelectedServerAccount { get; set; }

    #endregion
    
    #region Constructors

    public SavedServersVmd(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore,
        IHttpDataSc httpDataService,
        INavigationServices serverPageNavigationServices,
        IAuthorizationSc authorizationApiServices)
    {
        #region Store Initializing

        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;
        
        #endregion

        #region Services Initializing

        _httpDataSc = httpDataService;

        _serverPageNavigationServices = serverPageNavigationServices;

        _authorizationApiServices = authorizationApiServices;

        #endregion

        #region Subsriptions
        
         // _savedServersStore.WhenAnyValue(x => x.CurrentValue).Subscribe(x=> SavedServes = x);
         
         savedServersStore.CurrentValueChangedNotifier += () => SavedServes = savedServersStore?.CurrentValue;
         
        #endregion

        #region Commands Initializing

        DeleteServerCommand = ReactiveCommand.CreateFromTask(OnDeleteServerExecuted, CanDeleteServerExecute());

        ConnectToServerCommand =
            ReactiveCommand.CreateFromTask(OnConnectServerSavedExecuted, CanConnectServerSavedExecute());

        #endregion
    }

    #endregion

    #region Commands

    #region ConnectToServerCommand  : Connect to current selected server command

    /// <summary>
    ///     Connect to current selected server command
    /// </summary>
    public ICommand ConnectToServerCommand { get; }
    
    private IObservable<bool> CanConnectServerSavedExecute()
    {
        return this.WhenAnyValue(x =>
                x.SelectedServerAccount, 
            x => x._currentServerStore.CurrentServer,
            (selectedServerAccount, currentServer) =>
            {
                try
                {
                  return  selectedServerAccount?.SavedServer?.ApiIp != currentServer?.ApiIp;
                }
                catch 
                {
                    return false;
                }
            
            });
    }
    
    private async Task OnConnectServerSavedExecuted()
    {
        var serverStatus =
            await Task.Run(() => _httpDataSc.CheckServerStatus(SelectedServerAccount!.SavedServer!.ApiIp));

        if (!serverStatus) return;

        if (_currentServerStore.CurrentServer != default) _currentServerStore?.Delete();

        var newServerAccount = new Account
        {
            Login = SelectedServerAccount!.Account!.Login
        };

        try
        {
            await _authorizationApiServices!.Login(SelectedServerAccount.Account.IsAuthorized,
                SelectedServerAccount.Account, SelectedServerAccount.SavedServer!.ApiIp);

            newServerAccount = SelectedServerAccount.Account;
        }
        catch
        {
            await _authorizationApiServices!.Login(false, newServerAccount, SelectedServerAccount.SavedServer!.Ip);
        }

        _currentServerStore.CurrentServer = SelectedServerAccount.SavedServer;

        _serverPageNavigationServices.Navigate();
    }


    #endregion

    #region DeleteServerCommand : Delete current selected server command

    public ICommand DeleteServerCommand { get; }
    
    
    private Task OnDeleteServerExecuted()
    {
        try
        {
            _savedServersStore.RemoveFromEnumerable(SelectedServerAccount);

            SelectedServerAccount = null;
        }
        catch
        {
            //ignored
        }

        return Task.CompletedTask;
    }
    
    private IObservable<bool> CanDeleteServerExecute() => 
        this.WhenAnyValue(
            x => x.SelectedServerAccount,
            (serverAccount) =>
            {
                try
                {
                  return  _savedServersStore.Contains(serverAccount);
                }
                catch 
                {

                    return false;
                }
            }
        );


    #endregion

    #endregion

}