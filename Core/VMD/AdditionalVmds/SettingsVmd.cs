using System.Collections.ObjectModel;
using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation.Parametrize;
using AppInfrastructure.Stores.DefaultStore;
using Core.Models.AppInfrastructure.Menu;
using Core.Services.AppInfrastructure.NavigationServices.CloseServices;
using Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.VMD.AdditionalVmds.Base;
using Core.VMD.AdditionalVmds.SettingsVmds;
using Core.VMD.Base;
using Microsoft.Extensions.Configuration;
using ReactiveUI;

namespace Core.VMD.AdditionalVmds;

/// <summary>
///     Additional settings vmd
/// </summary>
public sealed class SettingsVmd : BaseAdditionalVmd
{
    public SettingsVmd(
        CloseAdditionalNavigationServices closeAdditionalNavigationServices,
        SettingsVmdsNavigationStore settingsVmdsNavigationStore,
        SettingsVmdsIocTypeNavigationService iocTypeNavigationService,
        IConfiguration configuration) : base(closeAdditionalNavigationServices)
    {

        #region Stores and Services Initializing

        _settingsVmdsNavigationStore = settingsVmdsNavigationStore;
        
        _settingsIocVmdsNavigationService = iocTypeNavigationService;

        #endregion

        #region Subscriptions

        _settingsVmdsNavigationStore.CurrentValueChangedNotifier +=
            () => this.RaisePropertyChanged(nameof(CurrentSettingsVmd));

        #endregion
     
        #region Command Initializing

        SettingsVmdsNavigationCommands = ReactiveCommand.Create<Type>(OnSettingsVmdsNavigate);

        #endregion
        
        #region Properties and Fileds Initializing

        _configuration = configuration;

        SettingsVMdsMenuItrems = new ObservableCollection<MenuItemWithCommand>
        {
            new ("Account",SettingsVmdsNavigationCommands,typeof(AccountSettingsVmd)),
            new ("Audio",SettingsVmdsNavigationCommands,typeof(AudioSettingsVmd)),
            new ("Saved servers",SettingsVmdsNavigationCommands,typeof(SavedServersSettingsVmd)),
            new ("About program",SettingsVmdsNavigationCommands,typeof(AboutProgramSettingsVmd))
          
            
        };

        #endregion


    }
    
    #region Properties and fields

    /// <summary>
    ///     Current app version
    /// </summary>
    public string Version => _configuration["AppSettings:AppVersions"] ?? "NonIdentify";

    /// <summary>
    ///     Current app branch
    /// </summary>
    public string Branch => _configuration["AppSettings:Branch"] ?? "NonIdentify";
    
    /// <summary>
    ///     appsettings.json 
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Current settings vmd
    /// </summary>
    public BaseVmd CurrentSettingsVmd => _settingsVmdsNavigationStore.CurrentValue;
    
    public ObservableCollection<MenuItemWithCommand> SettingsVMdsMenuItrems { get; }

    #endregion

    #region Commands

    #region _settingsVmdsNavigationCommands : Navigation command between settings vmds

   public ICommand SettingsVmdsNavigationCommands { get; }

    private void OnSettingsVmdsNavigate(Type NavigationType) => _settingsIocVmdsNavigationService.Navigate(NavigationType);

    #endregion
    
    #endregion

    #region Services

    private readonly IParamNavigationService<Type> _settingsIocVmdsNavigationService;

    #endregion

    #region Stores

    private readonly IStore<BaseVmd> _settingsVmdsNavigationStore;

    #endregion
}