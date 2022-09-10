using System.Collections.ObjectModel;
using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Stores.TemporaryInfo;
using Core.VMD.Base;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.Main.HubVmds;

/// <summary>
/// Current server manager vmd (Hub)
/// </summary>
public class CurrentServerManagerVmd : BaseVmd
{
    #region Stores
    
    [Reactive]
    private  SavedServersStore SavedServersStore { get; set; }
    
    private readonly CurrentServerAccountStore _currentServerAccountStore;
    
    #endregion
    
    #region Constructors

    public CurrentServerManagerVmd(
        INavigationServices openModalServerAuthorizationNavigationServices,
        INavigationServices openModalServerConnectionNavigationServices,
        SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore,
        CurrentServerAccountStore currentServerAccountStore)
    {

        #region Stores Initializing

        SavedServersStore = savedServersStore;
        
        _currentServerAccountStore = currentServerAccountStore;
        
        #endregion

        #region Subscrition

        savedServersStore.CurrentValueChangedNotifier += () =>
        {
            //Заметка: придумай как добавить в LiteApp поддержку INPC для совместимости с WhenAnyValue
            SavedServersStore = null;
            SavedServersStore = savedServersStore;
        };
        
        currentServerStore.CurrentServerChanged += 
            () => CurrentServer = currentServerStore.CurrentServer;
        
        currentServerStore.CurrentServerChanged +=
            () => CurrentServerIsNull = CurrentServer is null;
        
        #endregion
        
        #region Commands Initializing
        
        ModalServerConnectionCommand = new NavigationCommand(openModalServerConnectionNavigationServices);

        ModalRegistrationOpenCommand = new NavigationCommand(openModalServerAuthorizationNavigationServices,()=> (bool)!currentServerAccountStore?.CurrentValue?.IsAuthorized);
        
        DisconnectServerCommand = ReactiveCommand.CreateFromTask(_ => currentServerStore?.Delete());

        SaveServerCommand = ReactiveCommand.CreateFromTask(OnSaveServerCommandExecuted, CanSaveServerCommandExecute);

        #endregion
        
    }

    #endregion
    
    #region Properites and fields

    [Reactive] 
    public bool CurrentServerIsNull { get; private set; } = true;
    
    [Reactive]
    public Server CurrentServer { get; private set; }

    #endregion
    
    #region Commands

    #region Default commands

    /// <summary>
    ///     Open modalVmds with serverConnection
    /// </summary>
    public ICommand ModalServerConnectionCommand { get; }

    /// <summary>
    ///      Open modalVmds with Registration on currentServer
    /// </summary>
    public ICommand ModalRegistrationOpenCommand { get; }
    
    /// <summary>
    ///     Disconnect from current server
    /// </summary>
    public IReactiveCommand DisconnectServerCommand { get; }

    #endregion
    
    #region SaveServerCommand :  Save current server

    /// <summary>
    ///     Save current server
    /// </summary>
    public IReactiveCommand SaveServerCommand { get; }

    private IObservable<bool> CanSaveServerCommandExecute =>  
        this.WhenAnyValue(
            x=> x.CurrentServer,
            x=> x.SavedServersStore,
        (currentServer, savedServersStore) =>
        {
            try
            {
                return !savedServersStore.ContainsByServerApiIp(currentServer);
            }
            catch
            {
                return false;
            }
                
        });
    
    private async Task OnSaveServerCommandExecuted() =>
        SavedServersStore.AddIntoEnumerable(
            new ServerAccount
            {
                Account = _currentServerAccountStore.CurrentValue,
                SavedServer = CurrentServer
            });
    
    #endregion

    #endregion
}