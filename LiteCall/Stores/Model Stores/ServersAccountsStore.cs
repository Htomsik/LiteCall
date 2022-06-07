using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class ServersAccountsStore:BaseVMD
    {

        private static Dictionary<string, Account> DefaultDictionary = new Dictionary<string, Account>()
        {
            {"localhost:4999",new Account
            {
                Login = "JessJake",Password = "evilet228Q"
            }}
        };


        public event Action CurrentAccountChange;

        private void OnCurrentAccountChangeChanged()
        {
            CurrentAccountChange?.Invoke();
        }

        private Dictionary<string, Account> _SavedServerAccounts = DefaultDictionary;

        public Dictionary<string, Account> SavedServerAccounts
        {
            get => _SavedServerAccounts;
            set
            {
                Set(ref _SavedServerAccounts, value);
                OnCurrentAccountChangeChanged();
            }
        }


    }
}
