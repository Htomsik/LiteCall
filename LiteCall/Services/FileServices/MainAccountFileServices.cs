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

        private const string _FilePath = @"MainAccount.json";

        private AccountStore _AccountStore;

        private SettingsStore _SettingsStore;

        public MainAccountFileServices(AccountStore accountStore, SettingsStore settingsStore)
        {
            _AccountStore = accountStore;

            _SettingsStore = settingsStore;

            _AccountStore.CurrentAccountChange += SaveDataInFile;

            _SettingsStore.CurentSettingChanged += SaveDataInFile;
        }

        public async void GetDataFromFile()
        {
            try
            {
                var FileText = await File.ReadAllTextAsync(_FilePath);

                SavedMainAccount savedAccount = JsonConvert.DeserializeObject<SavedMainAccount>(FileText);

                if (savedAccount?._MainAccount != null)
                {
                    _AccountStore.CurrentAccount = savedAccount._MainAccount;
                }
                
                _SettingsStore.CurrentSettings = savedAccount._Settings;
            }
            catch (Exception e) { }
        }

        public async void SaveDataInFile()
        {
            try
            {

                var jsonNoNConvertedAccount =  _AccountStore.isDefaultAccount ? null : _AccountStore.CurrentAccount ;

                var jsonSerializeObject = JsonConvert.SerializeObject(new SavedMainAccount
                {
                    _MainAccount = jsonNoNConvertedAccount,
                    _Settings = _SettingsStore.CurrentSettings
                });

                await File.WriteAllTextAsync(_FilePath, jsonSerializeObject);
            }
            catch (Exception e) { }
        }
    }
}
