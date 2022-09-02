using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Models.AppInfrastructure;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;
using Microsoft.Extensions.Configuration;
using ReactiveUI;

namespace Core.VMD.Windows;

public sealed class MainWindowVmd : BaseVmd
{
    private readonly AdditionalVmdsNavigationStore _additionalVmdsNavigationStore;

    private readonly ICloseAppSc _closeAppSc;

    private readonly IConfiguration _configuration;

    private readonly MainWindowNavigationStore _mainWindowNavigationStore;

    private readonly ModalNavigationStore _modalNavigationStore;

    private readonly AppExecutionStateStore _statusMessageStore;


    public MainWindowVmd(MainWindowNavigationStore mainWindowNavigationStore,
        AdditionalVmdsNavigationStore additionalVmdsNavigationStore,
        ModalNavigationStore modalNavigationStore,
        AppExecutionStateStore statusMessageStore,
        INavigationSc closeModalNavigationScs,
        INavigationSc closeAdditionalNavigationSc,
        ICloseAppSc closeAppSc,
        IConfiguration configuration)
    {
        _mainWindowNavigationStore = mainWindowNavigationStore;

        _additionalVmdsNavigationStore = additionalVmdsNavigationStore;

        _modalNavigationStore = modalNavigationStore;

        _statusMessageStore = statusMessageStore;

        _closeAppSc = closeAppSc;

        _configuration = configuration;

        _mainWindowNavigationStore.CurrentValueChangedNotifier += OnCurrentViewModelChanged;

        _additionalVmdsNavigationStore.CurrentValueChangedNotifier += OnAdditionalVmdsCurrentViewModelChanged;

        _modalNavigationStore.CurrentValueChangedNotifier += OnModalCurrentViewModelChanged;

        _statusMessageStore.CurrentValueChangedNotifier += OnCurrentStatusMessageChanged;

        CloseModalCommand = new NavigationCommand(closeModalNavigationScs);

        CloseSettingsCommand = new NavigationCommand(closeAdditionalNavigationSc);
        
        CloseAppCommand = ReactiveCommand.CreateFromTask(OnCloseAppExecuted);
    }


    public string Version => _configuration["AppSettings:AppVersions"] ?? "NonIdentify";

    public string Branch => _configuration["AppSettings:Branch"] ?? "NonIdentify";


    public IReactiveCommand CloseAppCommand { get; }

    public ICommand CloseModalCommand { get; }

    public ICommand CloseSettingsCommand { get; }


    public BaseVmd? CurrentViewModel => _mainWindowNavigationStore.CurrentValue;

    public BaseVmd? ModalCurrentViewModel => _modalNavigationStore.CurrentValue;

    public BaseVmd? AdditionalCurrentViewModel => _additionalVmdsNavigationStore.CurrentValue;

    public AppExecutionState CurrentStatusMessage => _statusMessageStore.CurrentValue!;

    public bool AdditionalIsOpen => _additionalVmdsNavigationStore.CurrentValue is not null;

    public bool ModalIsOpen => _modalNavigationStore.CurrentValue is not null;

    public bool StatusMessageIsOpen =>  !string.IsNullOrEmpty(_statusMessageStore?.CurrentValue?.Message);

    private async Task OnCloseAppExecuted()
    {
        await _closeAppSc?.Close()!;
    }


    private void OnCurrentViewModelChanged()
    {
        this.RaisePropertyChanged(nameof(CurrentViewModel));
    }


    private void OnModalCurrentViewModelChanged()
    {
        this.RaisePropertyChanged(nameof(ModalCurrentViewModel));

        this.RaisePropertyChanged(nameof(ModalIsOpen));
    }

    private void OnAdditionalVmdsCurrentViewModelChanged()
    {
        this.RaisePropertyChanged(nameof(AdditionalCurrentViewModel));

        this.RaisePropertyChanged(nameof(AdditionalIsOpen));
    }

    private void OnCurrentStatusMessageChanged()
    {
        this.RaisePropertyChanged(nameof(CurrentStatusMessage));

        this.RaisePropertyChanged(nameof(StatusMessageIsOpen));
    }
}