using System.ComponentModel;
using System.Windows.Input;
using Core.VMD.Base;

namespace Core.VMD.Main.HubVmds.Base;

/// <summary>
///    Main Hub. It have navigations on all mains modules (Settings, server etc) 
/// </summary>
public interface IHubVmd : INotifyPropertyChanged
{
    #region Commands
    /// <summary>
    ///     Open AdditionalVmds with settings
    /// </summary>
    public ICommand OpenSettingsCommand { get; }

    #endregion
    
    #region Properties
    /// <summary>
    ///     Current saved servers management vmd
    /// </summary>
    public BaseVmd CurrentSavedServersVmd { get;}
    
    
    /// <summary>
    ///     Current server vmd
    /// </summary>
    public BaseVmd CurrentServerVmd { get; }

    #endregion
    
}