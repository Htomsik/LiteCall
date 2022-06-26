using System;
using LiteCall.Model;
using LiteCall.Model.Users;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class ServerAccountStore : BaseVmd
{
    private Account? _currentAccount;

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            Set(ref _currentAccount, value);
            OnCurrentAccountChangeChanged();
        }
    }

    public event Action? CurrentAccountChange;

    private void OnCurrentAccountChangeChanged()
    {
        CurrentAccountChange?.Invoke();
    }


    public void Logout()
    {
        CurrentAccount = new Account();
    }
}