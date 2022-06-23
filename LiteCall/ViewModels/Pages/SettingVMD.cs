using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace LiteCall.ViewModels.Pages;

internal sealed class SettingVmd : BaseVmd
{
    public SettingVmd(AccountStore? accountStore, SavedServersStore? savedServersStore, SettingsStore? settingsStore,
        INavigationService authNavigationService, IHttpDataServices httpDataServices, IStatusServices statusServices,
        SettingsAccNavigationStore settingsAccNavigationStore)
    {
        AccountStore = accountStore;

        SavedServersStore = savedServersStore;

        SettingsStore = settingsStore;

        _settingsAccNavigationStore = settingsAccNavigationStore;

        _authNavigationService = authNavigationService;

        _httpDataServices = httpDataServices;

        _statusServices = statusServices;

        LogoutAccCommand = new AccountLogoutCommand(accountStore);

        AccountStore!.CurrentAccountChange += AccountStatusChange;

        _settingsAccNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        AccountStatusChange();

        AddNewServerCommand = new AsyncLambdaCommand(OnAddNewServerExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
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
        set => Set(ref _inputDeviceses, value);
    }

    public ObservableCollection<string>? OutputDevices
    {
        get => _outputDevices;
        set => Set(ref _outputDevices, value);
    }

    public BaseVmd? AccountCurrentVmd => _settingsAccNavigationStore.SettingsAccCurrentViewModel;

    public ICommand LogoutAccCommand { get; }

    public ICommand AddNewServerCommand { get; }

    public string? NewServerApiIp
    {
        get => _newServerApiIp;
        set => Set(ref _newServerApiIp, value);
    }

    public string? NewSeverLogin
    {
        get => _newSeverLogin;
        set => Set(ref _newSeverLogin, value);
    }

    public bool IsDefault
    {
        get => _isDefault;
        set => Set(ref _isDefault, value);
    }

    public AccountStore? AccountStore
    {
        get => _accountStore;
        set => Set(ref _accountStore, value);
    }

    public SavedServersStore? SavedServersStore
    {
        get => _savedServersStore;
        set => Set(ref _savedServersStore, value);
    }

    public SettingsStore? SettingsStore
    {
        get => _settingsStore;
        set => Set(ref _settingsStore, value);
    }

    public int CaptureDeviceId
    {
        get => SettingsStore!.CurrentSettings!.CaptureDeviceId;
        set
        {
            Set(ref _inputDeviceId, value);

            SettingsStore!.CurrentSettings!.CaptureDeviceId = value;
        }
    }

    public int OutputDeviceId
    {
        get => SettingsStore!.CurrentSettings!.OutputDeviceId;
        set
        {
            Set(ref _outputDeviceId, value);

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
        OnPropertyChanged(nameof(AccountCurrentVmd));
    }

    private void AccountStatusChange()
    {
        IsDefault = AccountStore!.IsDefaultAccount;

        if (AccountStore.IsDefaultAccount)
            _authNavigationService.Navigate();
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
                _statusServices.ChangeStatus(new StatusMessage { Message = "Server already exists", IsError = true });
        }
    }

    #region Services

    private readonly INavigationService _authNavigationService;

    private readonly IHttpDataServices _httpDataServices;

    private readonly IStatusServices _statusServices;

    #endregion

    #region Stores

    private AccountStore? _accountStore;

    private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

    private SavedServersStore? _savedServersStore;

    private SettingsStore? _settingsStore;

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