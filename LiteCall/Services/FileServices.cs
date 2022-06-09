using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Newtonsoft.Json;

namespace LiteCall.Services
{
    internal class FileServices: IFileReadServices
    { 
        private  const string _FilePath = @"SavedServersAccounts.json";


        private ServersAccountsStore _ServersAccountsStore;

        public FileServices(ServersAccountsStore serversAccountsStore)
        {
            _ServersAccountsStore = serversAccountsStore;

            _ServersAccountsStore.ServersAccountsChange += SaveAccountServers;

         
        }

       

        public async void  GetAccountsServers()
       {

           try
           {
               var FileText = await File.ReadAllTextAsync(_FilePath);

               _ServersAccountsStore.SavedServerAccounts =
                   JsonConvert.DeserializeObject<ObservableCollection<ServerAccount>>(FileText);
           }
           catch (Exception e){}
           
           
       }

       public async void  SaveAccountServers()
       {
           try
           {
               var jsonSerializeObject = JsonConvert.SerializeObject(_ServersAccountsStore.SavedServerAccounts);

               await File.WriteAllTextAsync(_FilePath, jsonSerializeObject);
           }
           catch (Exception e){}
         
          
       }


    }
}
