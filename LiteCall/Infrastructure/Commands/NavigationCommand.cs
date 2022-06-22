using System;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Services.Interfaces;

namespace LiteCall.Infrastructure.Commands;

internal class NavigationCommand : BaseCommand
{
    private readonly Func<bool> _canExecute;

    private readonly INavigationService _navigationService;

    public NavigationCommand(INavigationService navigationService, Func<bool> CanExecute = null)
    {
        _navigationService = navigationService;
        _canExecute = CanExecute;
    }

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public override void Execute(object? parameter) => _navigationService.Navigate();
}