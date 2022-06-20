using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages;

internal class MainPageVMD : BaseVMD
{
    public MainPageVMD(AccountStore accountStore, ServerAccountStore serverAccountStore,
        ServersAccountsStore serversAccountsStore,
        CurrentServerStore currentServerStore,
        MainPageServerNavigationStore mainPageServerNavigationStore,
        INavigationService settingsPageNavigationService,
        INavigationService serverPageNavigationService,
        INavigationService openModalServerAuthorisationNavigationService,
        IAuthorisationServices authorisationApiServices,
        IStatusServices statusServices,
        IhttpDataServices httpDataServices)
    {
        AccountStore = accountStore;

        ServerAccountStore = serverAccountStore;

        ServersAccountsStore = serversAccountsStore;

        CurrentServerStore = currentServerStore;


        AuthorisationServices = authorisationApiServices;

        MainPageServerNavigationStore = mainPageServerNavigationStore;

        _serverPageNavigationService = serverPageNavigationService;

        _statusServices = statusServices;

        _httpDataServices = httpDataServices;


        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorisationNavigationService,
            CanModalRegistrationOpenCommandExecuted);


        VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);

        OpenModalCommaCommand = new LambdaCommand(OnOpenModalCommaExecuted);


        DisconnectServerCommand = new LambdaCommand(OnDisconnectServerExecuted, CanDisconnectServerExecute);

        AccountLogoutCommand = new LambdaCommand(OnAccountLogoutExecuted); //Не работает

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationService);


        SaveServerCommand = new AsyncLamdaCommand(OnSaveServerCommandExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
            CanSaveServerCommandExecute);

        DeleteServerSavedCommand = new AsyncLamdaCommand(OnDeleteServerSavedExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
            CanDeleteServerSavedExecute);


        ConnectServerCommand = new AsyncLamdaCommand(OnConnectServerExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }));

        ConnectServerSavedCommand = new AsyncLamdaCommand(OnConnectServerSavedExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
            CanConnectServerSavedExecute);


        DisconectServerReloader.Reloader += DisconectServer;

        MainPageServerNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
    }


    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(selectedViewModel));
    }

    private void DisconectServer()
    {
        if (selectedViewModel == null) return;

        selectedViewModel.Dispose();

        VisibilitiStatus = Visibility.Collapsed;

        MainPageServerNavigationStore.MainPageServerCurrentViewModel = null;
    }

    #region Команды

    public ICommand OpenSettingsCommand { get; }

    public ICommand ModalRegistrationOpenCommand { get; set; }

    private bool CanModalRegistrationOpenCommandExecuted()
    {
        if (ServerAccountStore.CurrentAccount != default) return !ServerAccountStore.CurrentAccount.IsAuthorise;

        return false;
    }


    public ICommand ConnectServerSavedCommand { get; }

    private bool CanConnectServerSavedExecute(object p)
    {
        if (CurrentServerStore.CurrentServer == default && SelectedServerAccount is not null)
            return SelectedServerAccount.SavedServer.ApiIp != CurrentServerStore.CurrentServer?.ApiIp;
        return false;
    }

    private async Task OnConnectServerSavedExecuted(object p)
    {
        // _statusServices.ChangeStatus(new StatusMessage { Message = "Check API server status. . ." });

        var ServerStatus =
            await Task.Run(() => _httpDataServices.CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp));

        if (!ServerStatus) return;

        if (selectedViewModel != default) DisconectServer();


        var newServerAccount = new Account
        {
            Login = SelectedServerAccount.Account.Login
        };


        try
        {
            var authoriseStatus = await AuthorisationServices.Login(SelectedServerAccount.Account.IsAuthorise,
                SelectedServerAccount.Account, SelectedServerAccount.SavedServer.ApiIp);

            if (authoriseStatus == 0)
            {
                _statusServices.ChangeStatus(new StatusMessage
                    { Message = "Authorization error. You will be logged without account", isError = true });

                await Task.Delay(1000);

                await AuthorisationServices.Login(false, newServerAccount, SelectedServerAccount.SavedServer.ApiIp);
            }
            else
            {
                newServerAccount = SelectedServerAccount.Account;
            }
        }
        catch (Exception e)
        {
            await AuthorisationServices.Login(false, newServerAccount, SelectedServerAccount.SavedServer.Ip);
        }


        ServerStatus =
            await Task.Run(() => _httpDataServices.CheckServerStatus(SelectedServerAccount.SavedServer.ApiIp));

        if (ServerStatus)
        {
            CurrentServerStore.CurrentServer = SelectedServerAccount.SavedServer;

            _serverPageNavigationService.Navigate();

            ServernameOrIp = string.Empty;

            VisibilitiStatus = Visibility.Visible;
        }
    }


    public ICommand SaveServerCommand { get; set; }

    private bool CanSaveServerCommandExecute(object p)
    {
        if (CurrentServerStore.CurrentServer == null) return false;

        try
        {
            return ServersAccountsStore.SavedServerAccounts.FirstOrDefault(x =>
                x.SavedServer.ApiIp == CurrentServerStore.CurrentServer.ApiIp) is null;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private async Task OnSaveServerCommandExecuted(object p)
    {
        ServersAccountsStore.Add(new ServerAccount
            { Account = ServerAccountStore.CurrentAccount, SavedServer = CurrentServerStore.CurrentServer });
    }


    public ICommand DeleteServerSavedCommand { get; }

    private bool CanDeleteServerSavedExecute(object p)
    {
        return SelectedServerAccount is not null;
    }

    private async Task OnDeleteServerSavedExecuted(object p)
    {
        ServersAccountsStore.Remove(SelectedServerAccount);
    }


    public ICommand DisconnectServerCommand { get; }

    private bool CanDisconnectServerExecute(object p)
    {
        return true;
    }

    private void OnDisconnectServerExecuted(object p)
    {
        DisconectServer();
    }


    public ICommand VisibilitySwitchCommand { get; }

    private void OnVisibilitySwitchExecuted(object p)
    {
        if (Convert.ToInt32(p) == 1)
            VisibilitiStatus = Visibility.Collapsed;
        else
            VisibilitiStatus = Visibility.Visible;
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

            ServernameOrIp = string.Empty;
        }
    }


    public ICommand AccountLogoutCommand { get; }

    private void OnAccountLogoutExecuted(object p)
    {
        if (selectedViewModel != null) MainPageServerNavigationStore.Close();

        CurrentServerStore.CurrentServer = default;

        VisibilitiStatus = Visibility.Collapsed;

        AccountStore.Logout();
    }


    public ICommand ConnectServerCommand { get; }

    private async Task OnConnectServerExecuted(object p)
    {
        var ServerAccount = new Account
        {
            Login = AccountStore.CurrentAccount.Login
        };

        Server newServer;

        string ApiIp;


        if (!CheckStatus)
        {
            //Получить информацию о сервере из главной базы по имени
            ApiIp = await _httpDataServices.MainServerGetApiIp(ServernameOrIp);

            if (ApiIp == null)
                //  _statusServices.DeleteStatus();

                return;

            newServer = await _httpDataServices.ApiServerGetInfo(ApiIp);

            if (newServer == null)
                //  _statusServices.DeleteStatus();

                return;

            newServer.ApiIp = ApiIp;
        }
        else
        {
            // Получить информацию из API сервера о сервере
            newServer = await _httpDataServices.ApiServerGetInfo(ServernameOrIp);

            if (newServer == null)
                // _statusServices.DeleteStatus();

                return;

            newServer.ApiIp = ServernameOrIp;
        }


        try
        {
            var DictionaryServerAccount =
                ServersAccountsStore.SavedServerAccounts.First(s => s.SavedServer.ApiIp == newServer.ApiIp.ToLower());

            var AuthoriseStatus = await AuthorisationServices.Login(DictionaryServerAccount.Account.IsAuthorise,
                DictionaryServerAccount.Account, newServer.ApiIp);

            if (AuthoriseStatus == 0)
            {
                MessageBox.Show("Authorization error. You will be logged without account", "Сообщение");

                await AuthorisationServices.Login(false, ServerAccount, newServer.ApiIp);
            }
            else
            {
                ServerAccount = DictionaryServerAccount.Account;
            }
        }
        catch (Exception e)
        {
            await AuthorisationServices.Login(false, ServerAccount, newServer.ApiIp);
        }


        //Заглушка на случай если Артём забудет убрать из сервера
        newServer.Ip = newServer.Ip.Replace("https://", "");

        var ServerStatus = await Task.Run(() => _httpDataServices.CheckServerStatus(newServer.Ip));

        if (newServer is not null && ServerStatus)
        {
            CurrentServerStore.CurrentServer = newServer;

            //    _statusServices.ChangeStatus(new StatusMessage { Message = "Сonnecting to server. . ." });

            await Task.Delay(250);

            ModalStatus = false;

            _serverPageNavigationService.Navigate();

            ServernameOrIp = string.Empty;

            VisibilitiStatus = Visibility.Visible;
        }
    }

    #endregion

    #region Данные

    private bool _CheckStatus;

    public bool CheckStatus
    {
        get => _CheckStatus;
        set => Set(ref _CheckStatus, value);
    }


    private bool _ModalStatus;

    public bool ModalStatus
    {
        get => _ModalStatus;
        set => Set(ref _ModalStatus, value);
    }


    private AccountStore _AccountStore;

    public AccountStore AccountStore
    {
        get => _AccountStore;
        set => Set(ref _AccountStore, value);
    }

    private CurrentServerStore _CurrentServerStore;

    public CurrentServerStore CurrentServerStore
    {
        get => _CurrentServerStore;
        set => Set(ref _CurrentServerStore, value);
    }


    private ServerAccountStore _ServerAccountStore;

    public ServerAccountStore ServerAccountStore
    {
        get => _ServerAccountStore;
        set => Set(ref _ServerAccountStore, value);
    }


    private ServerAccount _selectedServerAccount;

    public ServerAccount SelectedServerAccount
    {
        get => _selectedServerAccount;
        set => Set(ref _selectedServerAccount, value);
    }


    private ServersAccountsStore _ServersAccountsStore;

    public ServersAccountsStore ServersAccountsStore
    {
        get => _ServersAccountsStore;
        set => Set(ref _ServersAccountsStore, value);
    }


    private IAuthorisationServices _AuthorisationServices;

    public IAuthorisationServices AuthorisationServices
    {
        get => _AuthorisationServices;
        set => Set(ref _AuthorisationServices, value);
    }


    private string _servernameOrIp;

    public string ServernameOrIp
    {
        get => _servernameOrIp;
        set => Set(ref _servernameOrIp, value);
    }


    private Visibility _VisibilitiStatus = Visibility.Collapsed;

    public Visibility VisibilitiStatus
    {
        get => _VisibilitiStatus;
        set => Set(ref _VisibilitiStatus, value);
    }


    private readonly INavigationService _serverPageNavigationService;

    private readonly IStatusServices _statusServices;

    private readonly IhttpDataServices _httpDataServices;

    public MainPageServerNavigationStore MainPageServerNavigationStore;

    public BaseVMD selectedViewModel => MainPageServerNavigationStore.MainPageServerCurrentViewModel;

    #endregion
}