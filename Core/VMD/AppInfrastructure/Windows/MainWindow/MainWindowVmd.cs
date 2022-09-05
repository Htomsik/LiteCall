using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Close;
using AppInfrastructure.Stores.DefaultStore;
using Core.Infrastructure.CMD;
using Core.Models.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.AppInfrastructure.Windows.Base;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.AppInfrastructure.Windows.MainWindow;

public sealed class MainWindowVmd : BaseWindowVmdVmd
{
    #region Properties and Fields
    
    #region CurrentStatusMessage
    public AppExecutionState CurrentStatusMessage => _statusMessageStore.CurrentValue;
    
    private readonly AppExecutionStateStore _statusMessageStore;
    
    public bool StatusMessageIsOpen =>  !string.IsNullOrEmpty(_statusMessageStore?.CurrentValue?.Message);

    #endregion

    #region MainPageCurrentViewModel

    public BaseVmd? MainPageCurrentVmd => _mainWindowVmdNavigationStore.CurrentValue;
    
    private readonly IStore<BaseVmd> _mainWindowVmdNavigationStore;

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
    
    #endregion
    public MainWindowVmd(
        MainWindowVmdNavigationStore mainWindowVmdNavigationStore,
        AdditionalVmdsNavigationStore additionalVmdsNavigationStore,
        ModalVmdNavigationStore modalVmdNavigationStore,
        AppExecutionStateStore statusMessageStore,
        ICloseServices closeModalNavigationService,
        ICloseServices closeAppSc) : base(closeAppSc)
    {
        #region Stores and services Initializing

        _mainWindowVmdNavigationStore = mainWindowVmdNavigationStore;

        _additionalVmdsNavigationStore = additionalVmdsNavigationStore;

        _modalVmdNavigationStore = modalVmdNavigationStore;

        _statusMessageStore = statusMessageStore;

     
        
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
        
        #endregion
        
    }

    #region Commands
    
    /// <summary>
    ///     Close modal vmds
    /// </summary>
    public ICommand CloseModalCommand { get; }
    
    #endregion
    
}