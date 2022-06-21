using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class ServerAccountStore:BaseVMD
    {
        
        public event Action? CurrentAccountChange;

        private void OnCurrentAccountChangeChanged()
        {
            CurrentAccountChange?.Invoke();
        }

        private Account _currentAccount;

        public  Account CurrentAccount 
        {
            get => _currentAccount;
            set
            {
                Set(ref _currentAccount, value);
                OnCurrentAccountChangeChanged();
            }
        }


        public void Logout()
        {
            CurrentAccount = new Account();

        }
    }
}
