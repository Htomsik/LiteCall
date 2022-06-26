using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiteCall.Infrastructure.Bus;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages;

internal sealed class MainPageVmd : BaseVmd
{
    public MainPageVmd(AccountStore? accountStore, ServerAccountStore? serverAccountStore,
        SavedServersStore? savedServersStore,
        CurrentServerStore? currentServerStore,
        MainPageServerNavigationStore mainPageServerNavigationStore,
        INavigationService settingsPageNavigationService,
        INavigationService serverPageNavigationService,
        INavigationService openModalServerAuthorizationNavigationService,
        IAuthorizationServices? authorizationApiServices,
        IStatusServices statusServices,
        IHttpDataServices httpDataServices)
    {
        AccountStore = accountStore;

        ServerAccountStore = serverAccountStore;

        SavedServersStore = savedServersStore;

        CurrentServerStore = currentServerStore;


        _authorizationServices = authorizationApiServices;

        MainPageServerNavigationStore = mainPageServerNavigationStore;

        _serverPageNavigationService = serverPageNavigationService;

        _statusServices = statusServices;

        _httpDataServices = httpDataServices;


        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationService,
            CanModalRegistrationOpenCommandExecuted);


        VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);

        OpenModalCommaCommand = new LambdaCommand(OnOpenModalCommaExecuted);


        DisconnectServerCommand = new LambdaCommand(OnDisconnectServerExecuted);

        AccountLogoutCommand = new LambdaCommand(OnAccountLogoutExecuted); //Не работает

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationService);


        SaveServerCommand = new AsyncLambdaCommand(OnSaveServerCommandExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanSaveServerCommandExecute);

        DeleteServerSavedCommand = new AsyncLambdaCommand(OnDeleteServerSavedExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanDeleteServerSavedExecute);


        ConnectServerCommand = new AsyncLambdaCommand(OnConnectServerExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }));

        ConnectServerSavedCommand = new AsyncLambdaCommand(OnConnectServerSavedExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
            CanConnectServerSavedExecute);


        DisconnectServerReloader.Reloader += DisconnectServer;

        MainPageServerNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }


    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(SelectedViewModel));
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
        return !ServerAccountStore!.CurrentAccount!.IsAuthorized;
    }


    public ICommand ConnectServerSavedCommand { get; }

    private bool CanConnectServerSavedExecute(object p)
    {
        return SelectedServerAccount?.SavedServer?.ApiIp != CurrentServerStore?.CurrentServer?.ApiIp;
    }

    private async Task OnConnectServerSavedExecuted(object p)
    {
        var serverStatus =
            await Task.Run(() => _httpDataServices.CheckServerStatus(SelectedServerAccount!.SavedServer!.ApiIp));

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
                _statusServices.ChangeStatus(new StatusMessage
                    { Message = "Authorization error. You will be logged without account", IsError = true });

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
            await Task.Run(() => _httpDataServices.CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp));

        if (serverStatus)
        {
            CurrentServerStore!.CurrentServer = SelectedServerAccount.SavedServer;

            _serverPageNavigationService.Navigate();

            ServerNameOrIp = string.Empty;

            ButtonVisibleStatus = Visibility.Visible;
        }
    }


    public ICommand SaveServerCommand { get; set; }

    private bool CanSaveServerCommandExecute(object p)
    {
        return SavedServersStore?.SavedServerAccounts?.ServersAccounts?.FirstOrDefault(x =>
            x.SavedServer?.ApiIp == CurrentServerStore?.CurrentServer?.ApiIp) is null;
    }

    private Task OnSaveServerCommandExecuted(object p)
    {
        try
        {
            SavedServersStore!.Add(new ServerAccount
                { Account = ServerAccountStore!.CurrentAccount, SavedServer = CurrentServerStore!.CurrentServer });
        }
        catch
        {
            _statusServices.ChangeStatus(new StatusMessage
                { Message = "Serve save error", IsError = true });
        }

        return Task.CompletedTask;
    }


    public ICommand DeleteServerSavedCommand { get; }

    private bool CanDeleteServerSavedExecute(object p)
    {
        return SelectedServerAccount is not null;
    }

    private Task OnDeleteServerSavedExecuted(object p)
    {
        SavedServersStore!.Remove(SelectedServerAccount);
        return Task.CompletedTask;
    }


    public ICommand DisconnectServerCommand { get; }


    private void OnDisconnectServerExecuted(object p)
    {
        DisconnectServer();
    }


    public ICommand VisibilitySwitchCommand { get; }

    private void OnVisibilitySwitchExecuted(object p)
    {
        if (Convert.ToInt32(p) == 1)
            ButtonVisibleStatus = Visibility.Collapsed;
        else
            ButtonVisibleStatus = Visibility.Visible;
    }


    public ICommand OpenModalCommaCommand { get; }

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


    public ICommand AccountLogoutCommand { get; }

    private void OnAccountLogoutExecuted(object p)
    {
        if (SelectedViewModel != null) MainPageServerNavigationStore.Close();

        CurrentServerStore!.CurrentServer = default;

        ButtonVisibleStatus = Visibility.Collapsed;

        AccountStore!.Logout();
    }


    public ICommand ConnectServerCommand { get; }

    private async Task OnConnectServerExecuted(object p)
    {
        var serverAccount = new Account
        {
            Login = AccountStore!.CurrentAccount!.Login
        };

        Server? newServer;


        if (!CheckStatus)
        {
            var apiIp = await _httpDataServices.MainServerGetApiIp(ServerNameOrIp);

            if (apiIp == null)
                return;

            newServer = await _httpDataServices.ApiServerGetInfo(apiIp);

            if (newServer == null)
                return;

            newServer.ApiIp = apiIp;
        }
        else
        {
            newServer = await _httpDataServices.ApiServerGetInfo(ServerNameOrIp);

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
                _statusServices.ChangeStatus(new StatusMessage
                    { Message = "Authorization error. You will be logged without account", IsError = true });

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

        var serverStatus = await Task.Run(() => _httpDataServices.CheckServerStatus(newServer.Ip));

        if (serverStatus)
        {
            CurrentServerStore!.CurrentServer = newServer;

            await Task.Delay(250);

            ModalStatus = false;

            _serverPageNavigationService.Navigate();

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
        set => Set(ref _checkStatus, value);
    }


    private bool _modalStatus;

    public bool ModalStatus
    {
        get => _modalStatus;
        set => Set(ref _modalStatus, value);
    }


    private AccountStore? _accountStore;

    public AccountStore? AccountStore
    {
        get => _accountStore;
        set => Set(ref _accountStore, value);
    }

    private CurrentServerStore? _currentServerStore;

    public CurrentServerStore? CurrentServerStore
    {
        get => _currentServerStore;
        set => Set(ref _currentServerStore, value);
    }


    private ServerAccountStore? _serverAccountStore;

    public ServerAccountStore? ServerAccountStore
    {
        get => _serverAccountStore;
        set => Set(ref _serverAccountStore, value);
    }


    private ServerAccount? _selectedServerAccount;

    public ServerAccount? SelectedServerAccount
    {
        get => _selectedServerAccount;
        set => Set(ref _selectedServerAccount, value);
    }


    private SavedServersStore? _savedServersStore;

    public SavedServersStore? SavedServersStore
    {
        get => _savedServersStore;
        set => Set(ref _savedServersStore, value);
    }


    private string? _serverNameOrIp;

    public string? ServerNameOrIp
    {
        get => _serverNameOrIp;
        set => Set(ref _serverNameOrIp, value);
    }


    private Visibility _buttonVisibleStatus = Visibility.Collapsed;

    public Visibility ButtonVisibleStatus
    {
        get => _buttonVisibleStatus;
        set => Set(ref _buttonVisibleStatus, value);
    }


    private readonly INavigationService _serverPageNavigationService;

    private readonly IStatusServices _statusServices;

    private readonly IHttpDataServices _httpDataServices;


    private readonly IAuthorizationServices? _authorizationServices;

    public MainPageServerNavigationStore MainPageServerNavigationStore;

    public BaseVmd? SelectedViewModel => MainPageServerNavigationStore.MainPageServerCurrentViewModel;

    #endregion
}