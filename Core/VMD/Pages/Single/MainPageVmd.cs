using System.Windows;
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
using ReactiveUI;

namespace Core.VMD.Pages.Single;

public sealed class MainPageVmd : BaseVmd
{
    public MainPageVmd(MainAccountStore? accountStore, CurrentServerAccountStore? serverAccountStore,
        SavedServersStore? savedServersStore,
        CurrentServerStore? currentServerStore,
        MainPageServerNavigationStore mainPageServerNavigationStore,
        INavigationSc settingsPageNavigationSc,
        INavigationSc serverPageNavigationSc,
        INavigationSc openModalServerAuthorizationNavigationSc,
        IAuthorizationSc? authorizationApiServices,
        IStatusSc statusSc,
        IHttpDataSc httpDataSc)
    {
        AccountStore = accountStore;

        ServerAccountStore = serverAccountStore;

        SavedServersStore = savedServersStore;

        CurrentServerStore = currentServerStore;

        _authorizationServices = authorizationApiServices;

        MainPageServerNavigationStore = mainPageServerNavigationStore;

        _serverPageNavigationSc = serverPageNavigationSc;

        _statusSc = statusSc;

        _httpDataSc = httpDataSc;


        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationSc,
            CanModalRegistrationOpenCommandExecuted);
        
        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationSc);
        
        OpenModalCommand = ReactiveCommand.Create<object>(OnOpenModalCommaExecuted);
        
        DisconnectServerCommand = ReactiveCommand.CreateFromTask<object>(_ => CurrentServerStore?.Delete()!);
        
       SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted,CanSaveServerCommandExecute());
       
       DeleteServerSavedCommand = ReactiveCommand.CreateFromTask(OnDeleteServerSavedExecuted);
       
        ConnectServerCommand = ReactiveCommand.CreateFromTask(OnConnectServerExecuted);

     ConnectServerSavedCommand = ReactiveCommand.CreateFromTask(OnConnectServerSavedExecuted,CanConnectServerSavedExecute());


        DisconnectFromServerNotificator.Notificator += DisconnectServer;

        CurrentServerStore!.CurrentServerDeleted += DisconnectServer;

        MainPageServerNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }


    private void OnCurrentViewModelChanged()
    {
        this.RaisePropertyChanged(nameof(SelectedViewModel));
    }

    private void DisconnectServer()
    {
        if (SelectedViewModel == null) return;

        SelectedViewModel.Dispose();

        ButtonVisibleStatus = Visibility.Collapsed;

        MainPageServerNavigationStore.MainPageServerCurrentViewModel = null;
    }

    #region Команды

    public ICommand OpenSettingsCommand { get; }

    public ICommand ModalRegistrationOpenCommand { get; set; }

    private bool CanModalRegistrationOpenCommandExecuted()
    {
        return ServerAccountStore!.CurrentAccount!.IsAuthorized;
    }


    public IReactiveCommand ConnectServerSavedCommand { get; }
    
    private IObservable<bool> CanConnectServerSavedExecute() => this.WhenAnyValue(x => x.SelectedServerAccount,
        x => x.CurrentServerStore, (
            (account, store) => account?.SavedServer?.ApiIp != store?.CurrentServer?.ApiIp));

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
            var authorizeStatus = await _authorizationServices!.Login(SelectedServerAccount.Account.IsAuthorized,
                SelectedServerAccount.Account, SelectedServerAccount.SavedServer!.ApiIp);

            if (authorizeStatus == 0)
            {
                _statusSc.ChangeStatus(ExecutionErrorStates.AuthorizationFailed);

                await Task.Delay(1000);

                await _authorizationServices.Login(false, newServerAccount, SelectedServerAccount.SavedServer.ApiIp);
            }
            else
            {
                newServerAccount = SelectedServerAccount.Account;
            }
        }
        catch
        {
            await _authorizationServices!.Login(false, newServerAccount, SelectedServerAccount.SavedServer!.Ip);
        }


        serverStatus =
            await Task.Run(() => _httpDataSc.CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp));

        if (serverStatus)
        {
            CurrentServerStore!.CurrentServer = SelectedServerAccount.SavedServer;

            _serverPageNavigationSc.Navigate();

            ServerNameOrIp = string.Empty;

            ButtonVisibleStatus = Visibility.Visible;
        }
    }


    public IReactiveCommand SaveServerCommand { get; set; }
    
    private IObservable<bool> CanSaveServerCommandExecute() => this.WhenAnyValue(x=> x.SavedServersStore,
        (savedServersStore) =>
        {
            return savedServersStore?.SavedServerAccounts?.ServersAccounts?.FirstOrDefault(x =>
                x.SavedServer?.ApiIp == CurrentServerStore?.CurrentServer?.ApiIp) is null;
        });
    
    private Task OnSaveServerCommandExecuted()
    {
        try
        {
            SavedServersStore!.Add(new ServerAccount
                { Account = ServerAccountStore!.CurrentAccount, SavedServer = CurrentServerStore!.CurrentServer });
        }
        catch
        {
            _statusSc.ChangeStatus("Server save failed");
        }

        return Task.CompletedTask;
    }


    public IReactiveCommand DeleteServerSavedCommand { get; }

    
  

    private Task OnDeleteServerSavedExecuted()
    {
        SavedServersStore!.Remove(SelectedServerAccount);
        return Task.CompletedTask;
    }


    public IReactiveCommand DisconnectServerCommand { get; }
    
    public IReactiveCommand OpenModalCommand { get; }

    private void OnOpenModalCommaExecuted(object p)
    {
        if ((string)p == "1")
        {
            ModalStatus = true;
        }
        else
        {
            ModalStatus = false;

            ServerNameOrIp = string.Empty;
        }
     
    }
    public ICommand ConnectServerCommand { get; }

    private async Task OnConnectServerExecuted()
    {
        var serverAccount = new Account
        {
            Login = AccountStore!.CurrentAccount!.Login
        };

        Server? newServer;


        if (!CheckStatus)
        {
            var apiIp = await _httpDataSc.MainServerGetApiIp(ServerNameOrIp);

            if (apiIp == null)
                return;

            newServer = await _httpDataSc.ApiServerGetInfo(apiIp);

            if (newServer == null)
                return;

            newServer.ApiIp = apiIp;
        }
        else
        {
            newServer = await _httpDataSc.ApiServerGetInfo(ServerNameOrIp);

            if (newServer == null)
                return;

            newServer.ApiIp = ServerNameOrIp;
        }


        try
        {
            var dictionaryServerAccount =
                SavedServersStore!.SavedServerAccounts!.ServersAccounts!.First(s =>
                    s.SavedServer!.ApiIp == newServer.ApiIp!.ToLower());

            var authorizeStatus = await _authorizationServices!.Login(dictionaryServerAccount.Account!.IsAuthorized,
                dictionaryServerAccount.Account, newServer.ApiIp);

            if (authorizeStatus == 0)
            {
                _statusSc.ChangeStatus(ExecutionErrorStates.AuthorizationFailed);

                await _authorizationServices.Login(false, serverAccount, newServer.ApiIp);
            }
            else
            {
                serverAccount = dictionaryServerAccount.Account;
            }
        }
        catch
        {
            await _authorizationServices!.Login(false, serverAccount, newServer.ApiIp);
        }


        newServer.Ip = newServer.Ip!.Replace("https://", ""); //временно

        var serverStatus = await Task.Run(() => _httpDataSc.CheckServerStatus(newServer.Ip));

        if (serverStatus)
        {
            CurrentServerStore!.CurrentServer = newServer;

            await Task.Delay(250);

            ModalStatus = false;

            _serverPageNavigationSc.Navigate();

            ServerNameOrIp = string.Empty;

            ButtonVisibleStatus = Visibility.Visible;
        }
    }

    #endregion

    #region Данные

    private bool _checkStatus;

    public bool CheckStatus
    {
        get => _checkStatus;
        set => this.RaiseAndSetIfChanged(ref _checkStatus, value);
    }


    private bool _modalStatus;

    public bool ModalStatus
    {
        get => _modalStatus;
        set => this.RaiseAndSetIfChanged(ref _modalStatus, value);
    }


    private MainAccountStore? _accountStore;

    public MainAccountStore? AccountStore
    {
        get => _accountStore;
        set => this.RaiseAndSetIfChanged(ref _accountStore, value);
    }

    private CurrentServerStore? _currentServerStore;

    public CurrentServerStore? CurrentServerStore
    {
        get => _currentServerStore;
        set => this.RaiseAndSetIfChanged(ref _currentServerStore, value);
    }


    private CurrentServerAccountStore? _serverAccountStore;

    public CurrentServerAccountStore? ServerAccountStore
    {
        get => _serverAccountStore;
        set => this.RaiseAndSetIfChanged(ref _serverAccountStore, value);
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


    private string? _serverNameOrIp;

    public string? ServerNameOrIp
    {
        get => _serverNameOrIp;
        set => this.RaiseAndSetIfChanged(ref _serverNameOrIp, value);
    }


    private Visibility _buttonVisibleStatus = Visibility.Collapsed;

    public Visibility ButtonVisibleStatus
    {
        get => _buttonVisibleStatus;
        set => this.RaiseAndSetIfChanged(ref _buttonVisibleStatus, value);
    }


    private readonly INavigationSc _serverPageNavigationSc;

    private readonly IStatusSc _statusSc;

    private readonly IHttpDataSc _httpDataSc;


    private readonly IAuthorizationSc? _authorizationServices;

    public MainPageServerNavigationStore MainPageServerNavigationStore;

    public BaseVmd? SelectedViewModel => MainPageServerNavigationStore.MainPageServerCurrentViewModel;

    #endregion
}