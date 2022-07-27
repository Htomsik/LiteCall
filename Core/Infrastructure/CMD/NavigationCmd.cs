using Core.Infrastructure.CMD.Base;
using Core.Services.Interfaces.AppInfrastructure;

namespace Core.Infrastructure.CMD;

public sealed class NavigationCommand : CmdBase
{
    private readonly Func<bool>? _canExecute;

    private readonly INavigationSc _navigationSc;

    public NavigationCommand(INavigationSc navigationSc, Func<bool>? canExecute = null)
    {
        _navigationSc = navigationSc;
        
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public override void Execute(object? parameter)
    {
        _navigationSc.Navigate();
    }

    public override event EventHandler? CanExecuteChanged;
}