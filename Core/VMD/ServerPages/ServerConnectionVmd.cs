using AppInfrastructure.Services.NavigationServices.Close;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Models.Servers;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.ServerPages;

public class ServerConnectionVmd:BaseVmd
{
    private readonly IAuthorizationSc? _authorizationServices;

    private readonly SavedServersStore _savedServersStore;

    private readonly MainAccountStore? _accountStore;

    private readonly IHttpDataSc _httpDataSc;

    private readonly CurrentServerStore _currentServerStore;
    
    private readonly INavigationServices _serverPageNavigationServices;
    
    private readonly ICloseServices _closeModalNavigationServices;

    public ServerConnectionVmd(IAuthorizationSc? authorizationServices,
        SavedServersStore savedServersStore,
        MainAccountStore? accountStore,
        CurrentServerStore currentServerStore,
        IHttpDataSc httpDataSc, 
        INavigationServices serverPageNavigationServices,ICloseServices closeModalNavigationServices)
    {
        _authorizationServices = authorizationServices;
        
        _savedServersStore = savedServersStore;
        
        _accountStore = accountStore;
        
        _httpDataSc = httpDataSc;
        
        _currentServerStore = currentServerStore;
        
        _serverPageNavigationServices = serverPageNavigationServices;
        
        _closeModalNavigationServices = closeModalNavigationServices;

        ServerConnectCommand = ReactiveCommand.CreateFromTask(OnServerConnect,CanServerConnect);
        
    }

    public IReactiveCommand ServerConnectCommand { get; }

    public async Task OnServerConnect()
    {
        var serverAccount = new Account
        {
            Login = _accountStore!.CurrentValue!.Login
        };

        Server? newServer;

        try
        {
            var apiIp = !CheckStatus ? await _httpDataSc.MainServerGetApiIp(ServerNameOrIp) : ServerNameOrIp;

            newServer = await _httpDataSc.ApiServerGetInfo(apiIp);
        }
        catch (Exception)
        {
            return;
        }

        try
        {
            var dictionaryServerAccount =
                _savedServersStore!.SavedServerAccounts!.ServersAccounts!.First(s =>
                    s.SavedServer!.ApiIp == newServer!.ApiIp!.ToLower());

            await _authorizationServices!.Login(dictionaryServerAccount.Account!.IsAuthorized,
                dictionaryServerAccount.Account, newServer!.ApiIp);

            serverAccount = dictionaryServerAccount.Account;

        }
        catch
        {
            await _authorizationServices!.Login(false, serverAccount, newServer!.ApiIp);
        }

        var serverStatus = await Task.Run(() => _httpDataSc.CheckServerStatus(newServer.Ip));

        if (serverStatus)
        {
            _currentServerStore!.CurrentServer = newServer;
            
            _closeModalNavigationServices.Close();
            
            _serverPageNavigationServices.Navigate();

        }
    }

    private IObservable<bool> CanServerConnect => this.WhenAnyValue(x=>x.ServerNameOrIp,(serverNameOrIp)=> !String.IsNullOrEmpty(serverNameOrIp));



    #region Data

        private bool _checkStatus;

        public bool CheckStatus
        {
            get => _checkStatus;
            set => this.RaiseAndSetIfChanged(ref _checkStatus, value);
        }
        
        private string? _serverNameOrIp;

        public string? ServerNameOrIp
        {
            get => _serverNameOrIp;
            set => this.RaiseAndSetIfChanged(ref _serverNameOrIp, value);
        }


     #endregion
      
    }
