using System.Windows.Input;

namespace Core.Infrastructure.CMD.Base;

public abstract class AsyncCmdBase: ICommand
{
    private readonly Action<Exception> _onException;

    private bool _isExecuting;

    protected AsyncCmdBase(Action<Exception> onException)
    {
        _onException = onException;
    }

    protected bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public  event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter)
    {
        return !IsExecuting;
    }

    public  async void Execute(object? parameter)
    {
        IsExecuting = true;

        try
        {
            await ExecuteAsync(parameter!);
        }
        catch (Exception ex)
        {
            _onException?.Invoke(ex);
        }

        IsExecuting = false;
        
    }

    protected abstract Task ExecuteAsync(object parameter);
}