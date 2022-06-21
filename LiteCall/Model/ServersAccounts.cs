using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;
using Newtonsoft.Json;

namespace LiteCall.Model
{
    internal class ServerAccount
    {
        public Server SavedServer { get; set; }

        public Account Account { get; set; }
    }

    internal class SavedServers: AppSavedServers
    {
        public Account MainServerAccount { get; set; }
    }

    internal class AppSavedServers:BaseVMD  
    {

        [JsonIgnore]
        private ObservableCollection<ServerAccount> _serversAccounts = new ObservableCollection<ServerAccount>();

        public ObservableCollection<ServerAccount> ServersAccounts
        {
            get => _serversAccounts;
            set => Set(ref _serversAccounts, value);
        }


        public DateTime LastUpdated   { get; set; }
    }


}
