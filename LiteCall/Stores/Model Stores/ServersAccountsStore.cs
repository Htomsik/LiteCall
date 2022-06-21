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
               FindAccount = SavedServerAccounts.ServersAccounts?.First(x => x.SavedServer.ApiIp == newServerAccount.SavedServer.ApiIp);
           }
           catch (Exception e){}
           
           
           
           if (FindAccount == null)
           {
               if (SavedServerAccounts.ServersAccounts is null)
               {
                   SavedServerAccounts.ServersAccounts = new ObservableCollection<ServerAccount>();
               }

               SavedServerAccounts.ServersAccounts.Add(newServerAccount);
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
               FindAccount = SavedServerAccounts.ServersAccounts?.First(x => x.SavedServer.ApiIp == deletedServer.SavedServer.ApiIp);
           }
           catch (Exception e) { }


           if (FindAccount != null)
           {
               SavedServerAccounts.ServersAccounts?.Remove(FindAccount);
               OnCurrentSeverAccountChanged();
           }

       }

       public void Replace(Server ReplacedServer, Account newAccount)
       {

           ServerAccount? findAccount = null;


               try
               {
                   findAccount = SavedServerAccounts.ServersAccounts?.First(x => x.SavedServer.ApiIp == ReplacedServer.ApiIp);

                   SavedServerAccounts.ServersAccounts?.Remove(findAccount);

                   findAccount.Account = newAccount;

                   SavedServerAccounts.ServersAccounts?.Add(findAccount);
               }
               catch (Exception e)
               {
                   findAccount = new ServerAccount();

                   findAccount.Account = newAccount;

                   findAccount.SavedServer = ReplacedServer;

                   SavedServerAccounts.ServersAccounts?.Add(findAccount);
               }

            


           OnCurrentSeverAccountChanged();


       }

        private AppSavedServers _savedServerAccounts = new AppSavedServers{ServersAccounts = new ObservableCollection<ServerAccount>()};

        public AppSavedServers SavedServerAccounts 
        {
            get => _savedServerAccounts;
            set
            {
                Set(ref _savedServerAccounts, value);
                OnCurrentSeverAccountChanged();

            }
        }


    }
}
