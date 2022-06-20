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
    internal class MainAccountFileServices:IFileReadServices
    {

        private const string FilePath = $@"MainAccount.json";

        private readonly AccountStore _accountStore;

        private readonly SettingsStore _settingsStore;

        
        public MainAccountFileServices(AccountStore accountStore, SettingsStore settingsStore)
        {
            _accountStore = accountStore;

            _settingsStore = settingsStore;

            _accountStore.CurrentAccountChange += SaveDataInFile;

            _settingsStore.CurentSettingChanged += SaveDataInFile;
        }

        public async void GetDataFromFile()
        {
            try
            {
                var FileText =  File.ReadAllTextAsync(FilePath);

                SavedMainAccount savedAccount = JsonConvert.DeserializeObject<SavedMainAccount>(FileText.Result);

                if (savedAccount?._MainAccount != null)
                {
                    _accountStore.CurrentAccount = savedAccount._MainAccount;
                }
                
                _settingsStore.CurrentSettings = savedAccount._Settings;
            }
            catch (Exception e) { }
        }

        public async void SaveDataInFile()
        {
            try
            {

                var jsonNoNConvertedAccount =  _accountStore.isDefaultAccount ? null : _accountStore.CurrentAccount ;

                if (jsonNoNConvertedAccount is not null)
                {
                    jsonNoNConvertedAccount.Token = null;
                }
               

                var jsonSerializeObject = JsonConvert.SerializeObject(new SavedMainAccount
                {
                    _MainAccount = jsonNoNConvertedAccount,
                    _Settings = _settingsStore.CurrentSettings
                });

                await File.WriteAllTextAsync(FilePath, jsonSerializeObject);
            }
            catch (Exception e) { }
        }
    }
}
