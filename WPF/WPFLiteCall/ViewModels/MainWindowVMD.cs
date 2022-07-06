using System.Threading.Tasks;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Infrastructure.CMD.Lambda;
using Core.Models.AppInfrastructure;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.Base;
using Microsoft.Extensions.Configuration;

namespace LiteCall.ViewModels;

internal sealed class MainWindowVmd : BaseVmd
{
    private readonly AdditionalNavigationStore _additionalNavigationStore;

    private readonly ICloseAppSc _closeAppSc;

    private readonly IConfiguration _configuration;

    private readonly MainWindowNavigationStore _mainWindowNavigationStore;

    private readonly ModalNavigationStore _modalNavigationStore;

    private readonly AppExecutionStateStore _statusMessageStore;


    public MainWindowVmd(MainWindowNavigationStore mainWindowNavigationStore,
        AdditionalNavigationStore additionalNavigationStore,
        ModalNavigationStore modalNavigationStore,
        AppExecutionStateStore statusMessageStore,
        INavigationSc closeModalNavigationScs,
        INavigationSc closeAdditionalNavigationSc,
        IStatusSc statusSc,
        ICloseAppSc closeAppSc,
        IConfiguration configuration)
    {
        _mainWindowNavigationStore = mainWindowNavigationStore;

        _additionalNavigationStore = additionalNavigationStore;

        _modalNavigationStore = modalNavigationStore;

        _statusMessageStore = statusMessageStore;

        _closeAppSc = closeAppSc;

        _configuration = configuration;

        _mainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        _additionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

        _modalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

        _statusMessageStore.CurrentStatusMessageChanged += OnCurrentStatusMessageChanged;

        CloseModalCommand = new NavigationCommand(closeModalNavigationScs);

        CloseSettingsCommand = new NavigationCommand(closeAdditionalNavigationSc);

        CloseAppCommand = new AsyncLambdaCmd(OnCloseAppExecuted,
            ex => statusSc.ChangeStatus(ex.Message)
        );
    }


    public string Version => _configuration!.GetSection("AppSettings")["AppVersions"] ?? "0.1.0";

    public string Branch => _configuration!.GetSection("AppSettings")["Branch"] ?? "NonStable";


    public ICommand CloseAppCommand { get; }

    public ICommand CloseModalCommand { get; }

    public ICommand CloseSettingsCommand { get; }


    public BaseVmd? CurrentViewModel => _mainWindowNavigationStore.MainWindowCurrentViewModel;

    public BaseVmd? ModalCurrentViewModel => _modalNavigationStore.ModalMainWindowCurrentViewModel;

    public BaseVmd? AdditionalCurrentViewModel => _additionalNavigationStore.AdditionalMainWindowCurrentViewModel;

    public AppExecutionState CurrentStatusMessage => _statusMessageStore.CurrentStatusMessage!;

    public bool AdditionalIsOpen => _additionalNavigationStore.IsOpen;

    public bool ModalIsOpen => _modalNavigationStore.IsOpen;

    public bool StatusMessageIsOpen => _statusMessageStore.IsOpen;

    private async Task OnCloseAppExecuted(object p)
    {
        await _closeAppSc?.Close()!;
    }


    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }


    private void OnModalCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(ModalCurrentViewModel));

        OnPropertyChanged(nameof(ModalIsOpen));
    }

    private void OnAdditionalCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(AdditionalCurrentViewModel));

        OnPropertyChanged(nameof(AdditionalIsOpen));
    }

    private void OnCurrentStatusMessageChanged()
    {
        OnPropertyChanged(nameof(CurrentStatusMessage));

        OnPropertyChanged(nameof(StatusMessageIsOpen));
    }
}