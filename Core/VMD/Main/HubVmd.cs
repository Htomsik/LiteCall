﻿using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Retranslators.Base;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using Core.VMD.Main.HubVmds.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Main;

public sealed class HubVmd : BaseHubVmd
{
    public HubVmd(
        MainAccountStore mainAccountStore,
        CurrentServerAccountStore currentServerAccountStore,
        CurrentServerStore currentServerStore,
        CurrentServerVmdNavigationStore currentServerVmdNavigationStore,
        SavedServersStore savedServersStore,
        INavigationServices settingsPageNavigationServices,
        INavigationServices openModalServerAuthorizationNavigationServices,
        INavigationServices openModalServerConnectionNavigationServices,
        IRetranslor<Type, BaseVmd> iocRetranslator) 
        : base(settingsPageNavigationServices,
            currentServerVmdNavigationStore,
            iocRetranslator)
    {
        #region Store and services Initializing

        _mainMainAccountStore = mainAccountStore;

        _currentServerAccountStore = currentServerAccountStore;

        CurrentServerStore = currentServerStore;

        _savedServersStore = savedServersStore;
        
        #endregion

        #region Commands Initializing

        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationServices,()=> !_currentServerAccountStore.CurrentValue.IsAuthorized);

        ModalServerConnectionCommand = new NavigationCommand(openModalServerConnectionNavigationServices);
        
        DisconnectServerCommand = ReactiveCommand.CreateFromTask(_ => CurrentServerStore?.Delete()!);

        SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted, CanSaveServerCommandExecute());
        
        #endregion

        #region SubScription

        CurrentServerStore.CurrentServerDeleted += DisconnectFromServer;

        _currentServerAccountStore.CurrentValueChangedNotifier += () => this.RaisePropertyChanged(nameof(CurrentAccountInfo));
        
        CurrentServerStore.CurrentServerChanged += () => this.RaisePropertyChanged(nameof(CurrentServerIsNull));
        
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
    private readonly MainAccountStore _mainMainAccountStore;
    
    /// <summary>
    ///     Current server store
    /// </summary>
    [Reactive] 
    public CurrentServerStore? CurrentServerStore { get; set; }

    /// <summary>
    ///     Saved servers for current main account
    /// </summary>
    private readonly SavedServersStore _savedServersStore;

    #endregion
    
    #region Properties and Fields
    
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
    
    #endregion
    
    #region Commands
    
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
        return this.WhenAnyValue(x => x._savedServersStore, x=> x.CurrentServerStore,
            (savedServersStore, currentServerStore) => savedServersStore.ContainsByServerApiIp(currentServerStore.CurrentServer) );
    }

    private Task OnSaveServerCommandExecuted()
    {
        _savedServersStore.AddIntoEnumerable(new ServerAccount
            { Account = _currentServerAccountStore!.CurrentValue, SavedServer = CurrentServerStore!.CurrentServer });

        return Task.CompletedTask;
    }


    #endregion
    
    
    #endregion
    
}