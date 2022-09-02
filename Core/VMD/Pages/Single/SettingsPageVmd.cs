using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.Pages.Single;

public sealed class SettingsPageVmd : BaseVmd
{
    public SettingsPageVmd(MainAccountStore? accountStore, SavedServersStore? savedServersStore, AppSettingsStore? settingsStore,
        INavigationSc authNavigationSc, IHttpDataSc httpDataSc, IStatusSc statusSc,
        SettingsAccNavigationStore settingsAccNavigationStore)
    {
        AccountStore = accountStore;

        SavedServersStore = savedServersStore;

        SettingsStore = settingsStore;

        _settingsAccNavigationStore = settingsAccNavigationStore;

        _authNavigationSc = authNavigationSc;

        _httpDataSc = httpDataSc;

        _statusSc = statusSc;

        LogoutAccCommand = new AccountLogoutCmd(accountStore);

        AccountStore.CurrentValueChangedNotifier += AccountStatusChange;

        _settingsAccNavigationStore.CurrentValueChangedNotifier += OnCurrentViewModelChanged;

        AccountStatusChange();
        
        AddNewServerCommand = ReactiveCommand.CreateFromTask(OnAddNewServerExecuted, CanAddNewServerExecute());

        InputDevices = new ObservableCollection<string>();

        OutputDevices = new ObservableCollection<string>();

       
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

    public BaseVmd? AccountCurrentVmd => _settingsAccNavigationStore.CurrentValue;

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

    public bool IsDefault
    {
        get => _isDefault;
        set => this.RaiseAndSetIfChanged(ref _isDefault, value);
    }

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
        get => SettingsStore!.CurrentValue!.CaptureDeviceId;
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
    
    private void OnCurrentViewModelChanged()
    {
        ((IReactiveObject)this).RaisePropertyChanged(nameof(AccountCurrentVmd));
    }

    private void AccountStatusChange()
    {
        IsDefault = AccountStore!.IsDefaultAccount;

        if (AccountStore.IsDefaultAccount)
            _authNavigationSc.Navigate();
        else
            _settingsAccNavigationStore.Close();
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

    private readonly INavigationSc _authNavigationSc;

    private readonly IHttpDataSc _httpDataSc;

    private readonly IStatusSc _statusSc;

    #endregion

    #region Stores

    private MainAccountStore? _accountStore;

    private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

    private SavedServersStore? _savedServersStore;

    private AppSettingsStore? _settingsStore;

    #endregion

    #region Pivate fields

    private ObservableCollection<string>? _inputDeviceses;

    private bool _isDefault;

    private string? _newServerApiIp;

    private string? _newSeverLogin;

    private ObservableCollection<string>? _outputDevices;

    private int _outputDeviceId;

    private int _inputDeviceId;

    #endregion
}