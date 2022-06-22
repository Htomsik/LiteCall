using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiteCall.Infrastructure.Commands.Base;

internal abstract class AsyncCommandBase : BaseCommand
{
    private readonly Action<Exception> _onException;

    private bool _isExecuting;

    protected AsyncCommandBase(Action<Exception> onException)
    {
        _onException = onException;
    }

    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public new event EventHandler? CanExecuteChanged;

    public override bool CanExecute(object? parameter) => !IsExecuting;

    public override async void Execute(object? parameter)
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

        CommandManager.InvalidateRequerySuggested();
    }

    protected abstract Task ExecuteAsync(object parameter);
}