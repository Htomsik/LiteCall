using System;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal class MainWindowNavigationStore
{
    private BaseVmd? _MainWindowCurrentViewModel;

    public BaseVmd? MainWindowCurrentViewModel
    {
        get => _MainWindowCurrentViewModel;
        set
        {
            _MainWindowCurrentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}