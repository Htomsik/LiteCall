using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using ABI.System;

namespace LiteCall.ViewModels.Base;

internal abstract class BaseVmd : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        var handlers = PropertyChanged;

        if (handlers is null) return;

        var invocationList = handlers.GetInvocationList();

        var arg = new PropertyChangedEventArgs(propertyName);

        foreach (var action in invocationList)
            if (action.Target is DispatcherObject dispObject)
                dispObject.Dispatcher.Invoke(action, this, arg);
            else
                action.DynamicInvoke(this, arg);
    }


    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public virtual void Dispose()
    {
    }
}