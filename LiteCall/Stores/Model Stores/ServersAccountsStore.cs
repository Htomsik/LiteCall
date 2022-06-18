using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.ViewModels.Base;

namespace LiteCall.Stores
{
    internal class ServersAccountsStore:BaseVMD
    {


        public event Action ServersAccountsChange;

        private void OnCurrentSeverAccountChanged()
        {
            ServersAccountsChange?.Invoke();
        }


       public bool Add(ServerAccount newServerAccount)
       {

           ServerAccount FindAccount = null;

           try
           {
               FindAccount = SavedServerAccounts.First(x => x.SavedServer.ApiIp == newServerAccount.SavedServer.ApiIp);
           }
           catch (Exception e){}
           
           
           
           if (FindAccount == null)
           {
                 SavedServerAccounts.Add(newServerAccount);
                 OnCurrentSeverAccountChanged();
                 return true;
           }

           return false;

       }

       public void Remove(ServerAccount deletedServer)
       {

           ServerAccount FindAccount = null;

           try
           {
               FindAccount = SavedServerAccounts.First(x => x.SavedServer.ApiIp == deletedServer.SavedServer.ApiIp);
           }
           catch (Exception e) { }


           if (FindAccount != null)
           {
               SavedServerAccounts.Remove(FindAccount);
               OnCurrentSeverAccountChanged();
           }

       }

       public void Replace(Server ReplacedServer, Account newAccount)
       {

           ServerAccount FindAccount = null;


               try
               {
                   FindAccount = SavedServerAccounts.First(x => x.SavedServer.ApiIp == ReplacedServer.ApiIp);

                   SavedServerAccounts.Remove(FindAccount);

                   FindAccount.Account = newAccount;

                   SavedServerAccounts.Add(FindAccount);
               }
               catch (Exception e)
               {
                   FindAccount = new ServerAccount();

                   FindAccount.Account = newAccount;

                   FindAccount.SavedServer = ReplacedServer;

                   SavedServerAccounts.Add(FindAccount);
               }

            



           OnCurrentSeverAccountChanged();


        }

        private ObservableCollection<ServerAccount> _SavedServerAccounts = new ObservableCollection<ServerAccount>();

        public ObservableCollection<ServerAccount> SavedServerAccounts 
        {
            get => _SavedServerAccounts;
            set
            {
                Set(ref _SavedServerAccounts, value);
                OnCurrentSeverAccountChanged();
            }
        }


    }
}
