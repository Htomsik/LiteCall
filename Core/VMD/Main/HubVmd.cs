using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Retranslators.Base;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using Core.VMD.Main.HubVmds;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Main;


public sealed class HubVmd : BaseVmd
{
    public HubVmd(
        MainAccountStore mainAccountStore,
        CurrentServerAccountStore currentServerAccountStore,
        CurrentServerStore currentServerStore,
        CurrentServerVmdNavigationStore currentServerVmdNavigationStore,
        INavigationServices settingsPageNavigationServices,
        INavigationServices openModalServerAuthorizationNavigationServices,
        INavigationServices openModalServerConnectionNavigationServices,
        IRetranslor<Type, BaseVmd> iocRetranslator)
    {
        #region Store and services Initializing

        _mainMainAccountStore = mainAccountStore;

        _currentServerAccountStore = currentServerAccountStore;

        CurrentServerStore = currentServerStore;

        _currentServerVmdNavigationStore = currentServerVmdNavigationStore;


        _IocRetranslator = iocRetranslator;
        
        #endregion

        #region Commands Initializing

        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationServices,()=> !_currentServerAccountStore.CurrentValue.IsAuthorized);

        ModalServerConnectionCommand = new NavigationCommand(openModalServerConnectionNavigationServices);

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationServices);

        DisconnectServerCommand = ReactiveCommand.CreateFromTask(_ => CurrentServerStore?.Delete()!);

        SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted, CanSaveServerCommandExecute());
        
        #endregion

        #region SubScription

        CurrentServerStore.CurrentServerDeleted += DisconnectFromServer;

        _currentServerAccountStore.CurrentValueChangedNotifier += () => this.RaisePropertyChanged(nameof(CurrentAccountInfo));
        
        CurrentServerStore.CurrentServerChanged += () => this.RaisePropertyChanged(nameof(CurrentServerIsNull));

        _currentServerVmdNavigationStore.CurrentValueChangedNotifier += ()=>   this.RaisePropertyChanged(nameof(CurrentServerVmd));
        
        #endregion

        CurrentSavedServersVmd = _IocRetranslator.Retranslate(typeof(SavedServersVmd));
    }

    #region Stores

    /// <summary>
    ///     Current server Account
    /// </summary>
    private readonly CurrentServerAccountStore _currentServerAccountStore;
    
    /// <summary>
    ///     Current main account
    /// </summary>
    private readonly MainAccountStore _mainMainAccountStore;
    
    /// <summary>
    ///     Current server store
    /// </summary>
    [Reactive] 
    public CurrentServerStore? CurrentServerStore { get;  }
    
    /// <summary>
    ///     Saved servers for current main account
    /// </summary>
    [Reactive]
    public SavedServersStore? SavedServersStore { get; set; }

    #endregion

    #region Services

    private readonly IRetranslor<Type, BaseVmd> _IocRetranslator;

    #endregion
    
    #region Properties and Fields

    #region CurrentServerVmd

    private readonly CurrentServerVmdNavigationStore _currentServerVmdNavigationStore;

    public BaseVmd? CurrentServerVmd => _currentServerVmdNavigationStore.CurrentValue;

    #endregion
    
    #region CurrentAccountInfo : отображаемое текущая информаация об аккаунте

    public Account? CurrentAccountInfo => _currentServerAccountStore?.CurrentValue?.CurrentServerLogin is null 
        ? _mainMainAccountStore?.CurrentValue 
        : new Account
    {
        Login = _currentServerAccountStore.CurrentValue.CurrentServerLogin,
        IsAuthorized = _currentServerAccountStore.CurrentValue.IsAuthorized
    } ;
    
    
    #endregion
    
    public bool CurrentServerIsNull => CurrentServerStore?.CurrentServer is null;

    public BaseVmd CurrentSavedServersVmd { get; }

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
                return savedServersStore?.CurrentValue?.ServersAccounts?.FirstOrDefault(x =>
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
    
    
    #endregion
    
}