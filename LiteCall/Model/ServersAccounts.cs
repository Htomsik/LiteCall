using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteCall.Model
{
    internal class ServerAccount
    {
        public Server SavedServer { get; set; }

        public Account Account { get; set; }
    }

    internal class SavedServers
    {
        public ObservableCollection<ServerAccount> ServersAccounts { get; set; }

        public DateTime LastUpdated { get; set; }

        public Account MainServerAccount { get; set; }
    }

}
