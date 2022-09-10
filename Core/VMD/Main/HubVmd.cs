using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Models.Users;
using Core.Services.Retranslators.Base;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using Core.VMD.Main.HubVmds.Base;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Main;

public sealed class HubVmd : BaseHubVmd
{
    public HubVmd(
        MainAccountStore mainAccountStore,
        CurrentServerAccountStore currentServerAccountStore,
        CurrentServerStore currentServerStore,
        CurrentServerVmdNavigationStore currentServerVmdNavigationStore,
        INavigationServices settingsPageNavigationServices,
        IRetranslor<Type, BaseVmd> iocRetranslator) 
        : base(settingsPageNavigationServices,
            currentServerVmdNavigationStore,
            iocRetranslator)
    {
        
        #region SubScription

        currentServerStore.CurrentServerDeleted += DisconnectFromServer;

        currentServerAccountStore.CurrentValueChangedNotifier += () =>
            CurrentAccountInofoChanger(mainAccountStore.CurrentValue, currentServerAccountStore.CurrentValue);
        
        mainAccountStore.CurrentValueChangedNotifier += () =>
            CurrentAccountInofoChanger(mainAccountStore.CurrentValue, currentServerAccountStore.CurrentValue);
        
        #endregion

    }
    
    #region Properties and Fields
    
    #region CurrentAccountInfo : отображаемое текущая информаация об аккаунте

    [Reactive]
    public Account? CurrentAccountInfo { get; private set; }
    
    private void CurrentAccountInofoChanger(Account mainAccount, Account currentServeAccount) =>
        CurrentAccountInfo = currentServeAccount is null
            ? mainAccount
            : new Account
            {
                Login = currentServeAccount.Login,
                IsAuthorized = currentServeAccount.IsAuthorized
            }
    ;
    
    
    #endregion
    
    #endregion
    
}