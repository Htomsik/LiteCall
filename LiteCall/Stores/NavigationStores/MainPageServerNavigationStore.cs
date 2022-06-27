using System;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class MainPageServerNavigationStore
{
    private BaseVmd? _mainPageServerCurrentViewModel;

    public BaseVmd? MainPageServerCurrentViewModel
    {
        get => _mainPageServerCurrentViewModel;
        set
        {
            _mainPageServerCurrentViewModel?.Dispose();
            _mainPageServerCurrentViewModel = value;

            OnCurrentViewModelChanged();
        }
    }


    public void Close()
    {
        MainPageServerCurrentViewModel!.Dispose();
    }


    public event Action? CurrentViewModelChanged;

    private void OnCurrentViewModelChanged()
    {
        CurrentViewModelChanged?.Invoke();
    }
}