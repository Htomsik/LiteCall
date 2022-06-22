using System;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class AccountStore : BaseVmd
{
    private static readonly Account? DefaultAccount = new() { Login = "LC_User" };

    private Account? _currentAccount = DefaultAccount;

    public bool IsDefaultAccount => CurrentAccount == DefaultAccount;

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
        CurrentAccount = DefaultAccount;
    }
}