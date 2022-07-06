﻿using Core.Models.Users;
using Core.VMD.Base;

namespace Core.Stores.TemporaryInfo;

public sealed class CurrentServerAccountStore : BaseVmd
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