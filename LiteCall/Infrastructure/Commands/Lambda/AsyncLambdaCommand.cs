using System;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;

namespace LiteCall.Infrastructure.Commands.Lambda;

internal sealed class AsyncLambdaCommand : AsyncCommandBase
{
    private readonly Func<object, bool>? _canExecute;

    private readonly Func<object, Task> _execute;

    public AsyncLambdaCommand(Func<object, Task> execute, Action<Exception> onException,
        Func<object, bool>? canExecute = null) : base(onException)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    protected override async Task ExecuteAsync(object parameter)
    {
        await _execute(parameter);
    }

    public override bool CanExecute(object? parameter)
    {
        if (!IsExecuting)
            return _canExecute?.Invoke(parameter!) ?? true;
        return false;
    }
}