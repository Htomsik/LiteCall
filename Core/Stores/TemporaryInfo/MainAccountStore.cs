using Core.Models.Users;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.Stores.TemporaryInfo;

public class MainAccountStore:BaseVmd
{
    private readonly Account? _defaultAccount;

    private Account? _currentAccount;


    public MainAccountStore()
    {
        _defaultAccount = new Account { Login = "LC_User" };

        _currentAccount = _defaultAccount;
    }

    public bool IsDefaultAccount => CurrentAccount == _defaultAccount;

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentAccount, value);
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