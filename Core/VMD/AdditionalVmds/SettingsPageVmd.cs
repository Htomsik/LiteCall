using System.Collections.ObjectModel;
using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.AppInfrastructure.NavigationServices.CloseServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.AdditionalVmds.Base;
using Core.VMD.Base;
using Microsoft.Extensions.Configuration;
using ReactiveUI;

namespace Core.VMD.AdditionalVmds;

public sealed class SettingsPageVmd : BaseAdditionalVmd
{
    public SettingsPageVmd(MainAccountStore? accountStore,
        SavedServersStore? savedServersStore,
        AppSettingsStore? settingsStore,
        INavigationServices authNavigationServices,
        IHttpDataSc httpDataSc, 
        IStatusSc statusSc,
        SettingsAccountVmdNavigationStore settingsAccountVmdNavigationStore,
        CloseAdditionalNavigationServices closeAdditionalNavigationServices, 
        IConfiguration configuration ) : base(closeAdditionalNavigationServices)
    {

        #region Properties and Fileds Initializing

        _configuration = configuration;
        
        InputDevices = new ObservableCollection<string>();

        OutputDevices = new ObservableCollection<string>();

        #endregion
        
        #region Stores and Services  Initializing

        AccountStore = accountStore;

        SavedServersStore = savedServersStore;

        SettingsStore = settingsStore;
        
        _settingsAccountVmdNavigationStore = settingsAccountVmdNavigationStore;
        
        
        _authNavigationServices = authNavigationServices;

        _httpDataSc = httpDataSc;

        _statusSc = statusSc;


        #endregion
        
        #region Subscriptions

        AccountStore.CurrentValueChangedNotifier += AccountStatusChange;

        _settingsAccountVmdNavigationStore.CurrentValueChangedNotifier += ()=> this.RaisePropertyChanged(nameof(AccountCurrentVmd));

        #endregion
        
        #region Commands Initializnign

        AddNewServerCommand = ReactiveCommand.CreateFromTask(OnAddNewServerExecuted, CanAddNewServerExecute());
        
        LogoutAccCommand = new AccountLogoutBaseCmd(accountStore);

        #endregion
        
        AccountStatusChange();
       
    }

    public ObservableCollection<string>? InputDevices
    {
        get => _inputDeviceses;
        set => this.RaiseAndSetIfChanged(ref _inputDeviceses, value);
    }

    public ObservableCollection<string>? OutputDevices
    {
        get => _outputDevices;
        set => this.RaiseAndSetIfChanged(ref _outputDevices, value);
    }

    public BaseVmd? AccountCurrentVmd => _settingsAccountVmdNavigationStore.CurrentValue;

    public ICommand LogoutAccCommand { get; }

    public IReactiveCommand AddNewServerCommand { get; }

    public string? NewServerApiIp
    {
        get => _newServerApiIp;
        set => this.RaiseAndSetIfChanged(ref _newServerApiIp, value);
    }

    public string? NewSeverLogin
    {
        get => _newSeverLogin;
        set => this.RaiseAndSetIfChanged(ref _newSeverLogin, value);
    }

    public bool IsDefault => AccountStore.IsDefaultAccount;
    
    public MainAccountStore? AccountStore
    {
        get => _accountStore;
        set => this.RaiseAndSetIfChanged(ref _accountStore, value);
    }

    public SavedServersStore? SavedServersStore
    {
        get => _savedServersStore;
        set => this.RaiseAndSetIfChanged(ref _savedServersStore, value);
    }

    public AppSettingsStore? SettingsStore
    {
        get => _settingsStore;
        set => this.RaiseAndSetIfChanged(ref _settingsStore, value);
    }

    public int CaptureDeviceId
    {
        get => SettingsStore.CurrentValue.CaptureDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _inputDeviceId, value);

            SettingsStore!.CurrentValue!.CaptureDeviceId = value;
        }
    }

    public int OutputDeviceId
    {
        get => SettingsStore!.CurrentValue!.OutputDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _outputDeviceId, value);

            SettingsStore!.CurrentValue!.OutputDeviceId = value;
        }
    }
    
    private void AccountStatusChange()
    {
        this.RaisePropertyChanged(nameof(IsDefault));

        if (AccountStore.IsDefaultAccount)
            _authNavigationServices.Navigate();
        else
            _settingsAccountVmdNavigationStore.CurrentValue = default;
    }
 
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
                SavedServersStore!.Add(newSavedSeverAccount);
            }
            catch (Exception)
            {
                _statusSc.ChangeStatus("Server already exist");
            }
        
                
        }
    }

    #region Services

    private readonly INavigationServices _authNavigationServices;

    private readonly IHttpDataSc _httpDataSc;

    private readonly IStatusSc _statusSc;

    #endregion

    #region Stores

    private MainAccountStore? _accountStore;

    private readonly SettingsAccountVmdNavigationStore _settingsAccountVmdNavigationStore;

    private SavedServersStore? _savedServersStore;

    private AppSettingsStore? _settingsStore;

    #endregion

    #region Pivate fields

    private ObservableCollection<string>? _inputDeviceses;
    
    private string? _newServerApiIp;

    private string? _newSeverLogin;

    private ObservableCollection<string>? _outputDevices;

    private int _outputDeviceId;

    private int _inputDeviceId;

    #endregion

    
    #region Properties and fields

    /// <summary>
    ///     Current app version
    /// </summary>
    public string Version => _configuration["AppSettings:AppVersions"] ?? "NonIdentify";

    /// <summary>
    ///     Current app branch
    /// </summary>
    public string Branch => _configuration["AppSettings:Branch"] ?? "NonIdentify";
    
    /// <summary>
    ///     appsettings.json 
    /// </summary>
    private readonly IConfiguration _configuration;

    #endregion
}