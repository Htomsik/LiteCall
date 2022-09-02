using Core.Infrastructure.CMD.Base;
using Core.Stores.TemporaryInfo;

namespace Core.Infrastructure.CMD;

public class AccountLogoutBaseCmd:BaseCmd
{
  private readonly MainAccountStore? _currentAccountStore;

    public AccountLogoutBaseCmd(MainAccountStore? currentAccountStore)
    {
        _currentAccountStore = currentAccountStore;
    }

    public override bool CanExecute(object? parameter)
    {
        return true;
    }


    public override void Execute(object? parameter)
    {
        _currentAccountStore!.Logout();
    }

    public override event EventHandler? CanExecuteChanged;
}