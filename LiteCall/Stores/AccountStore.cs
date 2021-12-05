using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Stores
{
    public class AccountStore
    {
        private Account _CurrentAccount;
        public Account CurrentAccount
        {
            get => _CurrentAccount;
            set
            {
                _CurrentAccount = value;
            }
        }
    }
}
