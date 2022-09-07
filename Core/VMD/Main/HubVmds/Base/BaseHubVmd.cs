using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using AppInfrastructure.Stores.DefaultStore;
using Core.Infrastructure.CMD;
using Core.Services.Retranslators.Base;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.Main.HubVmds.Base;

/// <summary>
///     Base abstract realize for IHubVmd
/// </summary>
public abstract class BaseHubVmd : BaseVmd, IHubVmd
{
    #region Store

    private readonly IStore<BaseVmd>  _currentServerVmdNavigationStore;
    
    #endregion
    
    #region Properties and Fields

    public BaseVmd CurrentSavedServersVmd { get; }

    public BaseVmd CurrentServerVmd => _currentServerVmdNavigationStore.CurrentValue;
    
    
    
    #endregion
    
    #region Constructors

    public BaseHubVmd(INavigationServices settingsPageNavigationServices,
        IStore<BaseVmd> currentServerVmdNavigationStore,IRetranslor<Type, BaseVmd> iocRetranslator)
    {
        #region Stores and Services Initializing

        _currentServerVmdNavigationStore = currentServerVmdNavigationStore;

        #endregion

        #region Subscriptions

        _currentServerVmdNavigationStore.CurrentValueChangedNotifier += ()=>  ((IReactiveObject)this).RaisePropertyChanged(nameof(CurrentServerVmd));

        #endregion
        
        #region Commands Initializing

        OpenSettingsCommand = new NavigationCommand(settingsPageNavigationServices);

        #endregion
        
        //Dummy code. Think how refactor this. Maybe use local navigation store?
        CurrentSavedServersVmd = iocRetranslator.Retranslate(typeof(SavedServersVmd));
        
    }

    #endregion
    
    #region Commands

    public ICommand OpenSettingsCommand { get; }

    #endregion

    #region Methods

    /// <summary>
    ///     Invoke when disconnected from server
    /// </summary>
    protected void DisconnectFromServer()
    {
        if (CurrentServerVmd == null) return;

        CurrentServerVmd.Dispose();

        _currentServerVmdNavigationStore.CurrentValue = null;
    }


    #endregion
    
}