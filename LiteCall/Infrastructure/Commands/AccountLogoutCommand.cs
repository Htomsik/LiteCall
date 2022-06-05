using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Stores;

namespace LiteCall.Infrastructure.Commands
{
    internal class AccountLogoutCommand:BaseCommand
    {
        private AccountStore _CurrentAccountStore;
        public AccountLogoutCommand(AccountStore CurrentAccountStore) 
        {
            _CurrentAccountStore = CurrentAccountStore;
        }

        public override bool CanExecute(object parameter) => true;
       

        public override void Execute(object parameter)
        {
           _CurrentAccountStore.Logout();
        }
    }
}
