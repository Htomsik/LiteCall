using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores.ModelStores
{
    internal class CurrentServerStore:BaseVMD
    {
        public event Action CurrentServerChange;

        private void OnCurrentServerChangeChanged()
        {
            CurrentServerChange?.Invoke();
        }

        private Server _CurrentServer;

        public Server CurrentServer
        {
            get => _CurrentServer;
            set
            {
                Set(ref _CurrentServer, value);
                OnCurrentServerChangeChanged();
            }
        }


        public void Logout()
        {
            CurrentServer = null;

        }
    }
}
