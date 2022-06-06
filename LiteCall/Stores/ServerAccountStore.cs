using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class ServerAccountStore:AccountStore
    {
        

        public event Action CurrentAccountChange;

        private void OnCurrentAccountChangeChanged()
        {
            CurrentAccountChange?.Invoke();
        }

        private ServerAccount _CurrentAccount;

        public  ServerAccount CurrentAccount
        {
            get => _CurrentAccount;
            set
            {
                Set(ref _CurrentAccount, value);
                OnCurrentAccountChangeChanged();
            }
        }


        public void Logout()
        {
            CurrentAccount = null;

        }
    }
}
