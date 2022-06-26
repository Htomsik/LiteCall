using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Model.Errors;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using Microsoft.Extensions.Configuration;

namespace LiteCall.ViewModels;

internal sealed class MainWindowVmd : BaseVmd
{
    private readonly AdditionalNavigationStore _additionalNavigationStore;

    private readonly ICloseAppServices _closeAppServices;

    private readonly IConfiguration _configuration;

    private readonly MainWindowNavigationStore _mainWindowNavigationStore;

    private readonly ModalNavigationStore _modalNavigationStore;

    private readonly StatusMessageStore _statusMessageStore;


    public MainWindowVmd(MainWindowNavigationStore mainWindowNavigationStore,
        AdditionalNavigationStore additionalNavigationStore,
        ModalNavigationStore modalNavigationStore,
        StatusMessageStore statusMessageStore,
        INavigationService closeModalNavigationServices,
        INavigationService closeAdditionalNavigationService,
        IStatusServices statusServices,
        ICloseAppServices closeAppServices,
        IConfiguration configuration)
    {
        _mainWindowNavigationStore = mainWindowNavigationStore;

        _additionalNavigationStore = additionalNavigationStore;

        _modalNavigationStore = modalNavigationStore;

        _statusMessageStore = statusMessageStore;

        _closeAppServices = closeAppServices;

        _configuration = configuration;

        _mainWindowNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

        _additionalNavigationStore.CurrentViewModelChanged += OnAdditionalCurrentViewModelChanged;

        _modalNavigationStore.CurrentViewModelChanged += OnModalCurrentViewModelChanged;

        _statusMessageStore.CurrentStatusMessageChanged += OnCurrentStatusMessageChanged;

        CloseModalCommand = new NavigationCommand(closeModalNavigationServices);

        CloseSettingsCommand = new NavigationCommand(closeAdditionalNavigationService);

        CloseAppCommand = new AsyncLambdaCommand(OnCloseAppExecuted,
            ex => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message })
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

    public StatusMessage CurrentStatusMessage => _statusMessageStore.CurrentStatusMessage!;

    public bool AdditionalIsOpen => _additionalNavigationStore.IsOpen;

    public bool ModalIsOpen => _modalNavigationStore.IsOpen;

    public bool StatusMessageIsOpen => _statusMessageStore.IsOpen;

    private async Task OnCloseAppExecuted(object p)
    {
        await _closeAppServices?.Close()!;
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