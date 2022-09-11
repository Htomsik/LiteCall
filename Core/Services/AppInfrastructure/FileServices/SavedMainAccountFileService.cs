using Core.Models.Saved;
using Core.Services.AppInfrastructure.FileServices.Base;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using Newtonsoft.Json;

namespace Core.Services.AppInfrastructure.FileServices;

public sealed class SavedMainAccountFileService : IFileService
{
    private const string FilePath = @"MainAccount.json";

    private readonly MainAccountStore _accountStore;

    private readonly AppSettingsStore _settingsStore;


    public SavedMainAccountFileService(MainAccountStore accountStore, AppSettingsStore settingsStore)
    {
        _accountStore = accountStore;

        _settingsStore = settingsStore;

        _accountStore.CurrentValueChangedNotifier += SaveDataInFile;

        _settingsStore.CurrentValueChangedNotifier += SaveDataInFile;
    }

    public async void GetDataFromFile()
    {
        try
        {
            var fileText = await File.ReadAllTextAsync(FilePath);

            var savedAccount = JsonConvert.DeserializeObject<AppSavedMainAccount>(fileText);

            if (savedAccount?.MainAccount != null) _accountStore.CurrentValue = savedAccount.MainAccount;

            if (savedAccount?.Settings != null)
                _settingsStore.CurrentValue = savedAccount.Settings;
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
            var jsonNoNConvertedAccount = _accountStore.IsDefaultAccount ? null : _accountStore.CurrentValue;

            if (jsonNoNConvertedAccount is not null) jsonNoNConvertedAccount.Token = null;


            var jsonSerializeObject = JsonConvert.SerializeObject(new AppSavedMainAccount
            {
                MainAccount = jsonNoNConvertedAccount,
                Settings = _settingsStore.CurrentValue
            });

            await File.WriteAllTextAsync(FilePath, jsonSerializeObject);
        }
        catch
        {
            // ignored
        }
    }
}