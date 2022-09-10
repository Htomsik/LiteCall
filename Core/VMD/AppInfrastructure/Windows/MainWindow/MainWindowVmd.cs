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
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.AppInfrastructure.Windows.MainWindow;

public sealed class MainWindowVmd : BaseWindowVmd
{
    #region Properties and Fields
    
    #region CurrentStatusMessage
    
    [Reactive]
    public AppExecutionState CurrentStatusMessage { get; private set; }
    
    [Reactive]
    public bool StatusMessageIsOpen { get; private set; }

    #endregion
    
    [Reactive]
    public BaseVmd? MainPageCurrentVmd { get; private set; }

    #region ModalCurrentVmd

    [Reactive]
    public BaseVmd? ModalCurrentVmd { get; private set; }
    
    [Reactive]
    public bool ModalIsOpen { get; private set; }

    #endregion

    #region CurrentAdditionalVmd
    
    [Reactive]
    public BaseVmd? CurrentAdditionalVmd { get; private set; }
    
    [Reactive]
    public bool AdditionalIsOpen { get; private set; }

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
        
        #region Subscripptions

        mainWindowVmdNavigationStore.CurrentValueChangedNotifier += ()=>
            MainPageCurrentVmd = mainWindowVmdNavigationStore.CurrentValue;
        
        additionalVmdsNavigationStore.CurrentValueChangedNotifier += ()=>
        {
            CurrentAdditionalVmd = additionalVmdsNavigationStore.CurrentValue;

            AdditionalIsOpen = additionalVmdsNavigationStore.CurrentValue is not null;
        };
        
        modalVmdNavigationStore.CurrentValueChangedNotifier += () =>
        {
            ModalCurrentVmd = modalVmdNavigationStore.CurrentValue;

            ModalIsOpen = modalVmdNavigationStore.CurrentValue is not null;
        };
        
        
        statusMessageStore.CurrentValueChangedNotifier += () =>
        {
            CurrentStatusMessage = statusMessageStore.CurrentValue;

            StatusMessageIsOpen = !string.IsNullOrEmpty(statusMessageStore?.CurrentValue?.Message);
        };

        #endregion

        #region Properties Initializing

        //Mainpage
        
        MainPageCurrentVmd = mainWindowVmdNavigationStore.CurrentValue;
        
        //Additional
        
        CurrentAdditionalVmd = additionalVmdsNavigationStore.CurrentValue;

        AdditionalIsOpen = additionalVmdsNavigationStore.CurrentValue is not null;
        
        //Modal
        
        ModalCurrentVmd = modalVmdNavigationStore.CurrentValue;

        ModalIsOpen = modalVmdNavigationStore.CurrentValue is not null;
        
        //StatusMessage
        
        CurrentStatusMessage = statusMessageStore.CurrentValue;

        StatusMessageIsOpen = !string.IsNullOrEmpty(statusMessageStore?.CurrentValue?.Message);

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