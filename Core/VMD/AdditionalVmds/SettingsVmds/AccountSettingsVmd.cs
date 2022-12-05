using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Models.Users;
using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.AdditionalVmds.SettingsVmds.Base;
using Core.VMD.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.AdditionalVmds.SettingsVmds;

/// <summary>
///     Manage Account vmd in Settings vmd
/// </summary>
public class AccountSettingsVmd : BaseSettingsVmd
{

    #region Stores

    private readonly MainAccountStore _mainAccountStore;

    private readonly SettingsAccountManagementVmdNavigationStore _accountManagementVmdNavigationStore;

    #endregion

    #region Services

    private readonly IFullNavigationService _accountManagementNavigationService;

    #endregion
    
    #region Properties and Fields

    #region AccountData
    
    [Reactive]
    public bool IsDefaultAccount { get; private set; }

    [Reactive]
    public Account AcсountInfo { get; private set; }
    
    #endregion

    [Reactive]
    public BaseVmd CurrentAccountManagmentVmd { get; private set; }

    #endregion
    
    public AccountSettingsVmd(
        AppSettingsStore appSettingsStore,
        MainAccountStore mainAccountStore,
        SettingsAccountManagementVmdNavigationStore accountManagementVmdNavigationStore,
        IFullNavigationService accountManagementNavigationService) : base(appSettingsStore)
    {
        #region Stores and Servicces

        _accountManagementVmdNavigationStore = accountManagementVmdNavigationStore;

        _accountManagementNavigationService = accountManagementNavigationService;
        
        #endregion
        
        #region Properties and Fields Initializing

        _mainAccountStore = mainAccountStore;
        
        #endregion

        #region Subscriptions

        _mainAccountStore.CurrentValueChangedNotifier += () => OnMainAccountChanged();

        _accountManagementVmdNavigationStore.CurrentValueChangedNotifier += () =>
            CurrentAccountManagmentVmd = _accountManagementVmdNavigationStore.CurrentValue;
        
        #endregion

        #region Commands

        CurrentAccountLogoutCommand = ReactiveCommand.Create(() => _mainAccountStore.Logout());

        #endregion
        
        OnMainAccountChanged();
    }

    #region Commands

    /// <summary>
    ///     Account logout command
    /// </summary>
    public ICommand CurrentAccountLogoutCommand { get; }

    #endregion

    #region Sunbscription methods

    private void OnMainAccountChanged()
    {
        AcсountInfo = _mainAccountStore?.CurrentValue;

        IsDefaultAccount = _mainAccountStore.IsDefaultAccount;
        
        if (IsDefaultAccount)
            _accountManagementNavigationService.Navigate();
        else
            _accountManagementNavigationService.Close();
    }

    #endregion
}
