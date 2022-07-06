using Core.Models.AppInfrastructure;
using Core.VMD.Base;

namespace Core.Stores.AppInfrastructure;

public class AppSettingsStore:BaseVmd
{
    private AppSettings? _currentSettings = new();

    public AppSettingsStore()
    {
        CurrentSettings!.CurrentSettingsChanged += OnCurrentSettingsChanged;
    }

    public AppSettings? CurrentSettings
    {
        get => _currentSettings;
        set
        {
            Set(ref _currentSettings, value);
            OnCurrentSettingsChanged();
        }
    }

    public event Action? CurrentSettingsChanged;

    private void OnCurrentSettingsChanged()
    {
        CurrentSettingsChanged?.Invoke();
    }
}