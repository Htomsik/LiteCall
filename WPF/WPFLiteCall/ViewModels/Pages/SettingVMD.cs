using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Infrastructure.CMD.Lambda;
using Core.Models.Saved;
using Core.Models.Users;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using LiteCall.Services.Interfaces;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using ReactiveUI;

namespace LiteCall.ViewModels.Pages;

internal sealed class SettingVmd : BaseVmd
{
    public SettingVmd(MainAccountStore? accountStore, SavedServersStore? savedServersStore, AppSettingsStore? settingsStore,
        INavigationSc authNavigationSc, IHttpDataServices httpDataServices, IStatusSc statusSc,
        SettingsAccNavigationStore settingsAccNavigationStore)
    {
        AccountStore = accountStore;

        SavedServersStore = savedServersStore;

        SettingsStore = settingsStore;

        _settingsAccNavigationStore = settingsAccNavigationStore;

        _authNavigationSc = authNavigationSc;

        _httpDataServices = httpDataServices;

        _statusSc = statusSc;

        LogoutAccCommand = new AccountLogoutCmd(accountStore);

        AccountStore!.CurrentAccountChange += AccountStatusChange;

        _settingsAccNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        AccountStatusChange();

        AddNewServerCommand = new AsyncLambdaCmd(OnAddNewServerExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanAddNewServerExecute);


        InputDevices = new ObservableCollection<string>();

        OutputDevices = new ObservableCollection<string>();

        GetInputOutput();


        try
        {
            var capabilities = WaveIn.GetCapabilities(SettingsStore!.CurrentSettings!.CaptureDeviceId);
        }
        catch
        {
            SettingsStore!.CurrentSettings!.CaptureDeviceId = 0;
        }

        try
        {
            var capabilities = WaveOut.GetCapabilities(SettingsStore.CurrentSettings.OutputDeviceId);
        }
        catch
        {
            SettingsStore.CurrentSettings.OutputDeviceId = 0;
        }
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

    public BaseVmd? AccountCurrentVmd => _settingsAccNavigationStore.SettingsAccCurrentViewModel;

    public ICommand LogoutAccCommand { get; }

    public ICommand AddNewServerCommand { get; }

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
        get => SettingsStore!.CurrentSettings!.CaptureDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _inputDeviceId, value);

            SettingsStore!.CurrentSettings!.CaptureDeviceId = value;
        }
    }

    public int OutputDeviceId
    {
        get => SettingsStore!.CurrentSettings!.OutputDeviceId;
        set
        {
            this.RaiseAndSetIfChanged(ref _outputDeviceId, value);

            SettingsStore!.CurrentSettings!.OutputDeviceId = value;
        }
    }


    private async void GetInputOutput()
    {
        await Task.Run(() => InputDevices = AsyncGetInputDevices().Result);

        await Task.Run(() => OutputDevices = AsyncGetOutputDevices().Result);
    }

    private Task<ObservableCollection<string>?> AsyncGetInputDevices()
    {
        var inputDevices = new ObservableCollection<string>();

        var enumerator = new MMDeviceEnumerator();

        for (var waveInDevice = 0; waveInDevice < WaveIn.DeviceCount; waveInDevice++)
        {
            var deviceInfo = WaveIn.GetCapabilities(waveInDevice);

            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All))
                if (device.FriendlyName.StartsWith(deviceInfo.ProductName))
                    inputDevices.Add(device.FriendlyName);
        }


        return Task.FromResult(inputDevices)!;
    }

    private Task<ObservableCollection<string>?> AsyncGetOutputDevices()
    {
        var outputDevices = new ObservableCollection<string>();

        var enumerator = new MMDeviceEnumerator();

        for (var waveInDevice = 0; waveInDevice < WaveOut.DeviceCount; waveInDevice++)
        {
            var deviceInfo = WaveOut.GetCapabilities(waveInDevice);

            foreach (var device in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                outputDevices.Add(device.FriendlyName);
        }


        return Task.FromResult(outputDevices)!;
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

    private bool CanAddNewServerExecute(object p)
    {
        return !string.IsNullOrEmpty(NewServerApiIp) && !string.IsNullOrEmpty(NewSeverLogin);
    }

    private async Task OnAddNewServerExecuted(object p)
    {
        var newSavedSeverAccount = new ServerAccount
        {
            Account = new Account { Login = NewSeverLogin }
        };


        var serverStatus = await Task.Run(() => _httpDataServices.CheckServerStatus(NewServerApiIp));

        if (serverStatus)
        {
            var newServer = await _httpDataServices.ApiServerGetInfo(NewServerApiIp);

            if (newServer == null) return;

            newServer.ApiIp = NewServerApiIp;

            newSavedSeverAccount.SavedServer = newServer;

            if (!SavedServersStore!.Add(newSavedSeverAccount))
                _statusSc.ChangeStatus("Server already exist");
        }
    }

    #region Services

    private readonly INavigationSc _authNavigationSc;

    private readonly IHttpDataServices _httpDataServices;

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