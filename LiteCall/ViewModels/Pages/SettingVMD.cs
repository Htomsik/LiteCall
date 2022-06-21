using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using NAudio.Wave;

namespace LiteCall.ViewModels.Pages;

internal class SettingVMD : BaseVMD
{
    private readonly INavigationService _authNavigationService;

    private readonly IhttpDataServices _httpDataServices;


    private readonly SettingsAccNavigationStore _settingsAccNavigationStore;

   

    private readonly IStatusServices _statusServices;


    private AccountStore _accountStore;


    private ObservableCollection<string> _inputDevice;


    private bool _isDefault;

    private string _NewServerApiIp;


    private string _NewSeverLogin;


    private ObservableCollection<string> _outputDevice;


    private SavedServersStore _savedServersStore;


    public SettingVMD(AccountStore accountStore, SavedServersStore savedServersStore, SettingsStore settingsStore,
        INavigationService authNavigationService, IhttpDataServices httpDataServices, IStatusServices statusServices,
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

        AccountStore.CurrentAccountChange += AcoountStatusChange;

        _settingsAccNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        AcoountStatusChange();

        AddNewServerCommand = new AsyncLamdaCommand(OnAddNewServerExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { isError = true, Message = ex.Message }),
            CanAddNewServerExecute);


        inputDevice = new ObservableCollection<string>();

        outputDevice = new ObservableCollection<string>();

        for (var n = 0; n < WaveIn.DeviceCount; n++)
        {
            var capabilities = WaveIn.GetCapabilities(n);
            inputDevice.Add(capabilities.ProductName);
        }

        for (var n = 0; n < WaveOut.DeviceCount; n++)
        {
            var capabilities = WaveOut.GetCapabilities(n);
            outputDevice.Add(capabilities.ProductName);
        }


        try
        {
            var capabilities = WaveIn.GetCapabilities(settingsStore.CurrentSettings.CaptureDeviceId);
        }
        catch (Exception e)
        {
            settingsStore.CurrentSettings.CaptureDeviceId = 0;
        }

        try
        {
            var capabilities = WaveOut.GetCapabilities(settingsStore.CurrentSettings.OutputDeviceId);
        }
        catch (Exception e)
        {
            settingsStore.CurrentSettings.OutputDeviceId = 0;
        }
    }

    public ObservableCollection<string> inputDevice
    {
        get => _inputDevice;
        set => Set(ref _inputDevice, value);
    }

    public ObservableCollection<string> outputDevice
    {
        get => _outputDevice;
        set => Set(ref _outputDevice, value);
    }

    public BaseVMD AccountCurrentVMD => _settingsAccNavigationStore.SettingsAccCurrentViewModel;

    public ICommand LogoutAccCommand { get; }

    public ICommand AddNewServerCommand { get; }

    public string NewServerApiIp
    {
        get => _NewServerApiIp;
        set => Set(ref _NewServerApiIp, value);
    }

    public string NewSeverLogin
    {
        get => _NewSeverLogin;
        set => Set(ref _NewSeverLogin, value);
    }

    public bool IsDefault
    {
        get => _isDefault;
        set => Set(ref _isDefault, value);
    }

    public AccountStore AccountStore
    {
        get => _accountStore;
        set => Set(ref _accountStore, value);
    }

    public SavedServersStore SavedServersStore
    {
        get => _savedServersStore;
        set => Set(ref _savedServersStore, value);
    }


    private SettingsStore _SettingsStore;

    public SettingsStore SettingsStore
    {
        get => _SettingsStore;
        set => Set(ref _SettingsStore, value);
    }


    private int _CaptureDeviceId;

    public int CaptureDeviceId
    {
        get => SettingsStore.CurrentSettings.CaptureDeviceId;
        set
        {
            Set(ref _OutputDeviceId, value);

            SettingsStore.CurrentSettings.CaptureDeviceId = value;
        }
    }


    private int _OutputDeviceId;

    public int OutputDeviceId
    {
        get => SettingsStore.CurrentSettings.OutputDeviceId;
        set
        {
            Set(ref _OutputDeviceId, value);

            SettingsStore.CurrentSettings.OutputDeviceId = value;
        }
    }


    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(AccountCurrentVMD));
    }

    private void AcoountStatusChange()
    {
        IsDefault = AccountStore.isDefaultAccount;

        if (AccountStore.isDefaultAccount)
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

            if (!SavedServersStore.Add(newSavedSeverAccount))
                _statusServices.ChangeStatus(new StatusMessage { Message = "Server already exists", isError = true });
        }
    }
}