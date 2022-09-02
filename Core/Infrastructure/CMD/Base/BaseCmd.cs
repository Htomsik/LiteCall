using System.Windows.Input;


namespace Core.Infrastructure.CMD.Base;

public abstract class BaseCmd : ICommand
{
    public virtual event EventHandler? CanExecuteChanged;
    
    public abstract bool CanExecute(object? parameter);

    public abstract void Execute(object? parameter);
}