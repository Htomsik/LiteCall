using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Stores;

namespace LiteCall.Infrastructure.Commands;

internal sealed class AccountLogoutCommand : BaseCommand
{
    private readonly AccountStore? _currentAccountStore;

    public AccountLogoutCommand(AccountStore? currentAccountStore)
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
}