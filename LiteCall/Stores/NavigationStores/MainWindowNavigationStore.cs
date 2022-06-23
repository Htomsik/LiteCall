using System;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class MainWindowNavigationStore
{
    private BaseVmd? _mainWindowCurrentViewModel;

    public BaseVmd? MainWindowCurrentViewModel
    {
        get => _mainWindowCurrentViewModel;
        set
        {
            _mainWindowCurrentViewModel = value;
            OnCurrentViewModelChanged();
        }
    }

    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}