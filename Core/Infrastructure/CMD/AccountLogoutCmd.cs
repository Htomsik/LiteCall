using Core.Infrastructure.CMD.Base;
using Core.Stores.TemporaryInfo;

namespace Core.Infrastructure.CMD;

public class AccountLogoutCmd:CmdBase
{
  private readonly MainAccountStore? _currentAccountStore;

    public AccountLogoutCmd(MainAccountStore? currentAccountStore)
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