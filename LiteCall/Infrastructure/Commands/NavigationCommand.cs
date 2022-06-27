using System;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Services.Interfaces;

namespace LiteCall.Infrastructure.Commands;

internal sealed class NavigationCommand : BaseCommand
{
    private readonly Func<bool>? _canExecute;

    private readonly INavigationService _navigationService;

    public NavigationCommand(INavigationService navigationService, Func<bool>? canExecute = null)
    {
        _navigationService = navigationService;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public override void Execute(object? parameter)
    {
        _navigationService.Navigate();
    }
}