using System.Collections.ObjectModel;
using Core.Stores.AppInfrastructure;
using Core.VMD.AdditionalVmds.SettingsVmds.Base;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.VMD.AdditionalVmds.SettingsVmds;

/// <summary>
///      Manage Audio vmd in Settings vmd
/// </summary>
public class AudioSettingsVmd : BaseSettingsVmd
{
    #region Properties and Fields

    [Reactive]
    public ObservableCollection<string>? InputDevices { get; set; }
    
    [Reactive]
    public ObservableCollection<string>? OutputDevices { get; set; }

    /// <summary>
    ///     Current capture device
    /// </summary>
    public int CaptureDeviceId
    {
        get => _lazyAppSettingsStore.Value.CurrentValue.CaptureDeviceId;
        set =>  _lazyAppSettingsStore.Value.CurrentValue.CaptureDeviceId = value;
       
    }

    /// <summary>
    ///     Current output device
    /// </summary>
    public int OutputDeviceId
    {
        get => _lazyAppSettingsStore.Value.CurrentValue.OutputDeviceId;
        set => _lazyAppSettingsStore.Value.CurrentValue.OutputDeviceId = value;
    }

    #endregion
    
    public AudioSettingsVmd(AppSettingsStore appSettingsStore) : base(appSettingsStore)
    {
        #region Subscription

        _lazyAppSettingsStore.Value.CurrentValueChangedNotifier +=
            () =>
            {
                this.RaisePropertyChanged(nameof(CaptureDeviceId));
                this.RaisePropertyChanged(nameof(OutputDeviceId));
            };

        #endregion
    }
}