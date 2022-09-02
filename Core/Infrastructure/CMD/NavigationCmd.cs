using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD.Base;

namespace Core.Infrastructure.CMD;

public sealed class NavigationCommand : BaseCmd
{
    private readonly Func<bool>? _canExecute;

    private readonly INavigationServices _navigationServices;

    public NavigationCommand(INavigationServices navigationServices, Func<bool>? canExecute = null)
    {
        _navigationServices = navigationServices;
        
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke() ?? true;
    }

    public override void Execute(object? parameter)
    {
        _navigationServices.Navigate();
    }
    
}