﻿using Core.Infrastructure.CMD.Base;

namespace Core.Infrastructure.CMD.Lambda;


/// <summary>
/// Dont use it
/// </summary>
public class LambdaCmd:CmdBase
{
    private readonly Func<object, bool> _canExecute;
    private readonly Action<object> _execute;

    public LambdaCmd(Action<object> execute, Func<object, bool> canExecute = null!)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }


    public override bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter!) ?? true;
    }

    public override void Execute(object? parameter)
    {
        _execute(parameter!);
        // CommandManager.InvalidateRequerySuggested();
    }

    public override event EventHandler? CanExecuteChanged;
}