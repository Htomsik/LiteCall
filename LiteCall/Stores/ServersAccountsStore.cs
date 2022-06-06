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

        private static Dictionary<string, ServerAccount> DefaultDictionary = new Dictionary<string, ServerAccount>()
        {
            {"localhost:5002",new ServerAccount
            {
                Login = "JessJake",Password = "evilet228Q"
            }}
        };


        public event Action CurrentAccountChange;

        private void OnCurrentAccountChangeChanged()
        {
            CurrentAccountChange?.Invoke();
        }

        private Dictionary<string, ServerAccount> _SavedServerAccounts = DefaultDictionary;

        public Dictionary<string, ServerAccount> SavedServerAccounts
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
