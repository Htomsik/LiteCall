using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.MainVmds;


public sealed class MainPageVmd : BaseVmd
{
    public MainPageVmd(
        MainAccountStore accountStore,
        CurrentServerAccountStore currentServerAccountStore,
        SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore,
        CurrentServerVmdNavigationStore currentServerVmdNavigationStore,
        INavigationServices settingsPageNavigationServices,
        INavigationServices serverPageNavigationServices,
        INavigationServices openModalServerAuthorizationNavigationServices,
        INavigationServices openModalServerConnectionNavigationServices,
        IAuthorizationSc? authorizationApiServices,
        IStatusSc statusService,
        IHttpDataSc httpDataService)
    {
        #region Store and services Initializing

        _mainAccountStore = accountStore;

        _currentServerAccountStore = currentServerAccountStore;

        SavedServersStore = savedServersStore;

        CurrentServerStore = currentServerStore;

        _currentServerVmdNavigationStore = currentServerVmdNavigationStore;


        _serverPageNavigationServices = serverPageNavigationServices;

        _authorizationServices = authorizationApiServices;

        _statusService = statusService;

        _httpDataService = httpDataService;

        #endregion

        #region Commands Initializing

        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationServices,()=> !_currentServerAccountStore.CurrentValue.IsAuthorized);

        ModalServerConnectionCommand = new NavigationCommand(openModalServerConnectionNavigationServices);

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationServices);

        DisconnectServerCommand = ReactiveCommand.CreateFromTask(_ => CurrentServerStore?.Delete()!);

        SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted, CanSaveServerCommandExecute());

        DeleteSavedServerCommand =
            ReactiveCommand.CreateFromTask(OnDeleteServerSavedExecuted, CanDeleteServerSavedExecute());

        ConnectToSavedServerCommand =
            ReactiveCommand.CreateFromTask(OnConnectServerSavedExecuted, CanConnectServerSavedExecute());

        #endregion

        #region SubScription

        CurrentServerStore.CurrentServerDeleted += DisconnectFromServer;

        _currentServerAccountStore.CurrentValueChangedNotifier += () => this.RaisePropertyChanged(nameof(CurrentAccountInfo));
        
        CurrentServerStore.CurrentServerChanged += () => this.RaisePropertyChanged(nameof(CurrentServerIsNull));

        _currentServerVmdNavigationStore.CurrentValueChangedNotifier += ()=>   this.RaisePropertyChanged(nameof(CurrentServerVmd));
        
        #endregion
    }

    #region Stores

    /// <summary>
    ///     Current server Account
    /// </summary>
    private readonly CurrentServerAccountStore _currentServerAccountStore;
    
    /// <summary>
    ///     Current main account
    /// </summary>
    private readonly MainAccountStore _mainAccountStore;
    
    /// <summary>
    ///     Current server store
    /// </summary>
    [Reactive] 
    public CurrentServerStore? CurrentServerStore { get; set; }
    
    /// <summary>
    ///     Saved servers for current main account
    /// </summary>
    [Reactive]
    public SavedServersStore? SavedServersStore { get; set; }

    #endregion

    #region Services
    
    private readonly INavigationServices _serverPageNavigationServices;

    private readonly IStatusSc _statusService;

    private readonly IHttpDataSc _httpDataService;
    
    private readonly IAuthorizationSc? _authorizationServices;

    #endregion
    
    #region Properties and Fields

    #region CurrentServerVmd

    private readonly CurrentServerVmdNavigationStore _currentServerVmdNavigationStore;

    public BaseVmd? CurrentServerVmd => _currentServerVmdNavigationStore.CurrentValue;

    #endregion
    
    #region CurrentAccountInfo : отображаемое текущая информаация об аккаунте

    public Account? CurrentAccountInfo => _currentServerAccountStore?.CurrentValue?.CurrentServerLogin is null 
        ? _mainAccountStore?.CurrentValue 
        : new Account
    {
        Login = _currentServerAccountStore.CurrentValue.CurrentServerLogin,
        IsAuthorized = _currentServerAccountStore.CurrentValue.IsAuthorized
    } ;
    
    
    #endregion
    
    /// <summary>
    ///     Current selected saved server account
    /// </summary>
    [Reactive]
    public ServerAccount? SelectedServerAccount { get; set; }
    
    public bool CurrentServerIsNull => CurrentServerStore?.CurrentServer is null;
    
    #endregion

    #region Methods

    /// <summary>
    ///     Invoke when disconnected from server
    /// </summary>
    void DisconnectFromServer()
    {
        if (CurrentServerVmd == null) return;

        CurrentServerVmd.Dispose();

        _currentServerVmdNavigationStore.CurrentValue = null;
    }


    #endregion
    
    #region Commands

    /// <summary>
    ///     Open AdditionalVmds with settings
    /// </summary>
    public ICommand OpenSettingsCommand { get; }

    /// <summary>
    ///     Open modalVmds with serverConnection
    /// </summary>
    public ICommand ModalServerConnectionCommand { get; }

    /// <summary>
    ///      Open modalVmds with Registration on currentServer
    /// </summary>
    public ICommand ModalRegistrationOpenCommand { get; }
    
    /// <summary>
    ///     Disconnect from current server
    /// </summary>
    public IReactiveCommand DisconnectServerCommand { get; }

    #region ConnectToSavedServerCommand : Connect to selected saved server

    public IReactiveCommand ConnectToSavedServerCommand { get; }

    private IObservable<bool> CanConnectServerSavedExecute()
    {
        return this.WhenAnyValue(x => x.SelectedServerAccount,
            x => x.CurrentServerStore, (account, store) => account?.SavedServer?.ApiIp != store?.CurrentServer?.ApiIp);
    }
    
    private async Task OnConnectServerSavedExecuted()
    {
        var serverStatus =
            await Task.Run(() => _httpDataService.CheckServerStatus(SelectedServerAccount!.SavedServer!.ApiIp));

        if (!serverStatus) return;

        if (CurrentServerVmd != default) DisconnectFromServer();

        var newServerAccount = new Account
        {
            Login = SelectedServerAccount!.Account!.Login
        };

        try
        {
            await _authorizationServices!.Login(SelectedServerAccount.Account.IsAuthorized,
                SelectedServerAccount.Account, SelectedServerAccount.SavedServer!.ApiIp);

            newServerAccount = SelectedServerAccount.Account;
        }
        catch
        {
            await _authorizationServices!.Login(false, newServerAccount, SelectedServerAccount.SavedServer!.Ip);
        }

        CurrentServerStore!.CurrentServer = SelectedServerAccount.SavedServer;

        _serverPageNavigationServices.Navigate();
    }

    #endregion

    #region SaveServerCommand :  Save current server

    /// <summary>
    ///     Save current server
    /// </summary>
    public IReactiveCommand SaveServerCommand { get; }

    private IObservable<bool> CanSaveServerCommandExecute()
    {
        return this.WhenAnyValue(x => x.SavedServersStore,
            savedServersStore =>
            {
                return savedServersStore?.SavedServerAccounts?.ServersAccounts?.FirstOrDefault(x =>
                    x.SavedServer?.ApiIp == CurrentServerStore?.CurrentServer?.ApiIp) is null;
            });
    }

    private Task OnSaveServerCommandExecuted()
    {
        SavedServersStore!.Add(new ServerAccount
            { Account = _currentServerAccountStore!.CurrentValue, SavedServer = CurrentServerStore!.CurrentServer });

        return Task.CompletedTask;
    }


    #endregion
    
    #region DeleteSavedServerCommand : Delete selected saved server

    /// <summary>
    ///     Delete selected saved server
    /// </summary>
    public IReactiveCommand DeleteSavedServerCommand { get; }


    private Task OnDeleteServerSavedExecuted()
    {
        try
        {
            SavedServersStore!.Remove(SelectedServerAccount);

            SelectedServerAccount = null;
        }
        catch
        {
            //ignored
        }

        return Task.CompletedTask;
    }

    private IObservable<bool> CanDeleteServerSavedExecute()
    {
        return this.WhenAnyValue(x => x.SavedServersStore,
            savedServersStore =>
            {
                return savedServersStore?.SavedServerAccounts?.ServersAccounts?.FirstOrDefault(x =>
                    x.SavedServer?.ApiIp == SelectedServerAccount?.SavedServer!.ApiIp) is not null;
            });
    }

    #endregion
    
    #endregion
    
}