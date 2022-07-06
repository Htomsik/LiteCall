using Core.Infrastructure.CMD.Base;

namespace Core.Infrastructure.CMD.Lambda;

public sealed class AsyncLambdaCmd:AsyncCmdBase
{
    private readonly Func<object, bool>? _canExecute;

    private readonly Func<object, Task> _execute;

    public AsyncLambdaCmd(Func<object, Task> execute, Action<Exception> onException,
        Func<object, bool>? canExecute = null) : base(onException)
    {
        _execute = execute;

        _canExecute = canExecute;
    }

    protected override async Task ExecuteAsync(object parameter)
    {
        await _execute(parameter);
    }

    public override  bool CanExecute(object? parameter)
    {
        if (!IsExecuting)
            return _canExecute?.Invoke(parameter!) ?? true;
        return false;
    }
}
