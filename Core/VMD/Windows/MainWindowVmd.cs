using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Close;
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

    #region Properties and Fields
    
    #region CurrentStatusMessage
    public AppExecutionState CurrentStatusMessage => _statusMessageStore.CurrentValue;
    
    private readonly AppExecutionStateStore _statusMessageStore;
    
    public bool StatusMessageIsOpen =>  !string.IsNullOrEmpty(_statusMessageStore?.CurrentValue?.Message);

    #endregion

    #region MainPageCurrentViewModel

    public BaseVmd? MainPageCurrentVmd => _mainWindowVmdNavigationStore.CurrentValue;
    
    private readonly MainWindowVmdNavigationStore _mainWindowVmdNavigationStore;

    #endregion

    #region ModalCurrentVmd

    public BaseVmd? ModalCurrentVmd => _modalVmdNavigationStore.CurrentValue;
    
    private readonly ModalVmdNavigationStore _modalVmdNavigationStore;
    public bool ModalIsOpen => _modalVmdNavigationStore.CurrentValue is not null;

    #endregion

    #region CurrentAdditionalVmd
    
    public BaseVmd? CurrentAdditionalVmd => _additionalVmdsNavigationStore.CurrentValue;

    private readonly AdditionalVmdsNavigationStore _additionalVmdsNavigationStore;
    
    public bool AdditionalIsOpen => _additionalVmdsNavigationStore.CurrentValue is not null;

    #endregion
    
    public string Version => _configuration["AppSettings:AppVersions"] ?? "NonIdentify";

    public string Branch => _configuration["AppSettings:Branch"] ?? "NonIdentify";
    
    private readonly ICloseAppSc _closeAppSc;

    private readonly IConfiguration _configuration;
    
    
    #endregion
    
    public MainWindowVmd(
        MainWindowVmdNavigationStore mainWindowVmdNavigationStore,
        AdditionalVmdsNavigationStore additionalVmdsNavigationStore,
        ModalVmdNavigationStore modalVmdNavigationStore,
        AppExecutionStateStore statusMessageStore,
        ICloseServices closeModalNavigationService,
        ICloseServices closeAdditionalNavigationServices,
        ICloseAppSc closeAppSc,
        IConfiguration configuration)
    {
        #region Stora and services Initializing

        _mainWindowVmdNavigationStore = mainWindowVmdNavigationStore;

        _additionalVmdsNavigationStore = additionalVmdsNavigationStore;

        _modalVmdNavigationStore = modalVmdNavigationStore;

        _statusMessageStore = statusMessageStore;

        _closeAppSc = closeAppSc;

        _configuration = configuration;


        #endregion
        
        #region Subscripptions

        _mainWindowVmdNavigationStore.CurrentValueChangedNotifier += ()=> this.RaisePropertyChanged(nameof(MainPageCurrentVmd));
        
        _additionalVmdsNavigationStore.CurrentValueChangedNotifier += ()=> 
        { 
            this.RaisePropertyChanged(nameof(CurrentAdditionalVmd));

            this.RaisePropertyChanged(nameof(AdditionalIsOpen));
        };
        
        _modalVmdNavigationStore.CurrentValueChangedNotifier += () =>
        {
            this.RaisePropertyChanged(nameof(ModalCurrentVmd));

            this.RaisePropertyChanged(nameof(ModalIsOpen));
        };
        
        
        _statusMessageStore.CurrentValueChangedNotifier += () =>
        {
            this.RaisePropertyChanged(nameof(CurrentStatusMessage));

            this.RaisePropertyChanged(nameof(StatusMessageIsOpen));
        };

        #endregion

        #region Command Initializing 

        CloseModalCommand = new CloseNavigationCmd(closeModalNavigationService);

        CloseSettingsCommand = new CloseNavigationCmd(closeAdditionalNavigationServices);
        
        CloseAppCommand = ReactiveCommand.CreateFromTask(()=>  _closeAppSc?.Close());

        #endregion
        
    }

    #region Commands

    /// <summary>
    ///     Close Application 
    /// </summary>
    public IReactiveCommand CloseAppCommand { get; }

    /// <summary>
    ///     Close modal vmds
    /// </summary>
    public ICommand CloseModalCommand { get; }

    /// <summary>
    ///     Close SettingsVmds
    /// </summary>
    public ICommand CloseSettingsCommand { get; }

    #endregion


}