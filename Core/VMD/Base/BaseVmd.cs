using System.Runtime.CompilerServices;
using ReactiveUI;


namespace Core.VMD.Base;

public class BaseVmd:ReactiveObject
{
   
    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null!)
    {
        this.RaiseAndSetIfChanged(ref field, value);
        return true;
    }

    public virtual void Dispose()
    {
    }
}