using System.IO;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Newtonsoft.Json;

namespace LiteCall.Services.FileServices;

internal class MainAccountFileServices : IFileReadServices
{
    private const string FilePath = @"MainAccount.json";

    private readonly AccountStore _accountStore;

    private readonly SettingsStore _settingsStore;


    public MainAccountFileServices(AccountStore accountStore, SettingsStore settingsStore)
    {
        _accountStore = accountStore;

        _settingsStore = settingsStore;

        _accountStore.CurrentAccountChange += SaveDataInFile;

        _settingsStore.CurrentSettingsChanged += SaveDataInFile;
    }

    public async void GetDataFromFile()
    {
        try
        {
            var fileText = await File.ReadAllTextAsync(FilePath);

            var savedAccount = JsonConvert.DeserializeObject<SavedMainAccount>(fileText);

            if (savedAccount?.MainAccount != null) _accountStore.CurrentAccount = savedAccount.MainAccount;

            if (savedAccount?.Settings != null)
                _settingsStore.CurrentSettings = savedAccount.Settings;
        }
        catch
        {
            // ignored
        }
    }

    public async void SaveDataInFile()
    {
        try
        {
            var jsonNoNConvertedAccount = _accountStore.IsDefaultAccount ? null : _accountStore.CurrentAccount;

            if (jsonNoNConvertedAccount is not null) jsonNoNConvertedAccount.Token = null;


            var jsonSerializeObject = JsonConvert.SerializeObject(new SavedMainAccount
            {
                MainAccount = jsonNoNConvertedAccount,
                Settings = _settingsStore.CurrentSettings
            });

            await File.WriteAllTextAsync(FilePath, jsonSerializeObject);
        }
        catch
        {
            // ignored
        }
    }
}