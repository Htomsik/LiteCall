﻿using System.Windows;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Infrastructure.Notifiers;
using Core.Models.AppInfrastructure.StateStatuses;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Pages.Single;

public sealed class MainPageVmd : BaseVmd
{
    public MainPageVmd(MainAccountStore? accountStore, CurrentServerAccountStore? currentServerAccountStore,
        SavedServersStore? savedServersStore,
        CurrentServerStore? currentServerStore,
        MainPageServerNavigationStore mainPageServerNavigationStore,
        INavigationSc settingsPageNavigationSc,
        INavigationSc serverPageNavigationSc,
        INavigationSc openModalServerAuthorizationNavigationSc,
        INavigationSc openModalServerConnectionNavigationSc,
        IAuthorizationSc? authorizationApiServices,
        IStatusSc statusSc,
        IHttpDataSc httpDataSc)
    {
        AccountStore = accountStore;

        CurrentServerAccountStore = currentServerAccountStore;

        SavedServersStore = savedServersStore;

        CurrentServerStore = currentServerStore;

        _authorizationServices = authorizationApiServices;

        _mainPageServerNavigationStore = mainPageServerNavigationStore;

        _serverPageNavigationSc = serverPageNavigationSc;

        _statusSc = statusSc;

        _httpDataSc = httpDataSc;


        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationSc,
            CanModalRegistrationOpenCommandExecuted);

        ModalServerConnectionCommand = new NavigationCommand(openModalServerConnectionNavigationSc);

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationSc);
        
        DisconnectServerCommand = ReactiveCommand.CreateFromTask(_ => CurrentServerStore?.Delete()!);

        SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted, CanSaveServerCommandExecute());

        DeleteSavedServerCommand = ReactiveCommand.CreateFromTask(OnDeleteServerSavedExecuted,CanDeleteServerSavedExecute());
        
        ConnectServerSavedCommand =
            ReactiveCommand.CreateFromTask(OnConnectServerSavedExecuted, CanConnectServerSavedExecute());
        
        CurrentServerStore!.CurrentServerDeleted += DisconnectServer;

        CurrentServerStore!.CurrentServerChanged += CurrentServerChanged;

        _mainPageServerNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        this.WhenAnyPropertyChanged();

    }

    private void CurrentServerChanged()
    {
        this.RaisePropertyChanged(nameof(ButtonVisibleStatus));
    }


    private void OnCurrentViewModelChanged()
    {
        this.RaisePropertyChanged(nameof(SelectedViewModel));
    }

    private void DisconnectServer()
    {
        if (SelectedViewModel == null) return;

        SelectedViewModel.Dispose();
        
        _mainPageServerNavigationStore.MainPageServerCurrentViewModel = null;
    }

    #region Команды

    public ICommand OpenSettingsCommand { get; }
    
    public ICommand ModalServerConnectionCommand { get; }

    public ICommand ModalRegistrationOpenCommand { get;}

    private bool CanModalRegistrationOpenCommandExecuted()
    {
        return CurrentServerAccountStore!.CurrentAccount!.IsAuthorized;
    }
    
    public IReactiveCommand ConnectServerSavedCommand { get; }

    private IObservable<bool> CanConnectServerSavedExecute()
    {
        return this.WhenAnyValue(x => x.SelectedServerAccount,
            x => x.CurrentServerStore, (account, store) => account?.SavedServer?.ApiIp != store?.CurrentServer?.ApiIp);
    }

    private async Task OnConnectServerSavedExecuted()
    {
        var serverStatus =
            await Task.Run(() => _httpDataSc.CheckServerStatus(SelectedServerAccount!.SavedServer!.ApiIp));

        if (!serverStatus) return;

        if (SelectedViewModel != default) DisconnectServer();
        
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

        _serverPageNavigationSc.Navigate();
        
    }


    public IReactiveCommand SaveServerCommand { get;}

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
                { Account = CurrentServerAccountStore!.CurrentAccount, SavedServer = CurrentServerStore!.CurrentServer });
            
        return Task.CompletedTask;
    }


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
    


    public IReactiveCommand DisconnectServerCommand { get; }
    
    #endregion

    #region Данные
    
    private MainAccountStore? _accountStore;

    public MainAccountStore? AccountStore
    {
        get => _accountStore;
        set => this.RaiseAndSetIfChanged(ref _accountStore, value);
    }
    
    [Reactive]
    public CurrentServerStore? CurrentServerStore { get; set; }
   

    private CurrentServerAccountStore? _currentServerAccountStore;

    public CurrentServerAccountStore? CurrentServerAccountStore
    {
        get => _currentServerAccountStore;
        set => this.RaiseAndSetIfChanged(ref _currentServerAccountStore, value);
    }


    private ServerAccount? _selectedServerAccount;

    public ServerAccount? SelectedServerAccount
    {
        get => _selectedServerAccount;
        set => this.RaiseAndSetIfChanged(ref _selectedServerAccount, value);
    }


    private SavedServersStore? _savedServersStore;

    public SavedServersStore? SavedServersStore
    {
        get => _savedServersStore;
        set => this.RaiseAndSetIfChanged(ref _savedServersStore, value);
    }


    public bool ButtonVisibleStatus => CurrentServerStore!.CurrentServer is null;
    
    private readonly INavigationSc _serverPageNavigationSc;

    private readonly IStatusSc _statusSc;

    private readonly IHttpDataSc _httpDataSc;


    private readonly IAuthorizationSc? _authorizationServices;

    private readonly MainPageServerNavigationStore _mainPageServerNavigationStore;

    public BaseVmd? SelectedViewModel => _mainPageServerNavigationStore.MainPageServerCurrentViewModel;

    #endregion
}