using System;
using LiteCall.Model;
using LiteCall.Model.Users;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores;

internal sealed class AccountStore : BaseVmd
{
    private readonly Account? _defaultAccount;


    public AccountStore()
    {
        _defaultAccount =  new Account { Login = "LC_User" };

        _currentAccount = _defaultAccount;
    }

    private Account? _currentAccount;

    public bool IsDefaultAccount => CurrentAccount == _defaultAccount;

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
        CurrentAccount = _defaultAccount;
    }
}