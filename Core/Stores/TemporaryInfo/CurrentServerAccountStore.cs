using Core.Models.Users;
using Core.VMD.Base;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Stores.TemporaryInfo;

public sealed class CurrentServerAccountStore : BaseVmd
{
    
    [Reactive]
    public Account? CurrentAccount { get; set; }
    
    
    
    
    
    public void Logout()
    {
        CurrentAccount = new Account();
    }
}