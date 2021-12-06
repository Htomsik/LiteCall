using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class AccountStore:BaseVMD
    {
      


        private Account _CurrentAccount;

        public Account CurrentAccount
        {
            get => _CurrentAccount;
            set => Set(ref _CurrentAccount, value);
        }


        public void Logout()
        {
            _CurrentAccount=null;
        }
    }
}
