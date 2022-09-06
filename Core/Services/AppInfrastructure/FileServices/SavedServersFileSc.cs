using System.Collections.ObjectModel;
using Core.Models.Saved;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.TemporaryInfo;
using Newtonsoft.Json;

namespace Core.Services.AppInfrastructure.FileServices;

public sealed class SavedServersFileSc : IFileSc
{
    private const string FilePath = @"SavedServersAccounts.json";

    private readonly MainAccountStore _accountStore;


    private readonly SavedServersStore _savedServersStore;

    public SavedServersFileSc(SavedServersStore savedServersStore, MainAccountStore accountStore)
    {
        _savedServersStore = savedServersStore;

        _accountStore = accountStore;

        _savedServersStore.CurrentValueChangedNotifier += SaveDataInFile;

        _accountStore.CurrentValueChangedNotifier += GetDataFromFile;
    }


    public async void GetDataFromFile()
    {
        try
        {
            var fileText = await File.ReadAllTextAsync(FilePath);

            var allUsers = JsonConvert.DeserializeObject<List<CurrentAccountSavedServers>>(fileText);

            var currentUserServerStore = allUsers!.Find(s =>
                s.MainServerAccount!.IsAuthorized! == _accountStore!.CurrentValue!.IsAuthorized! &&
                s!.MainServerAccount!.Login! == _accountStore!.CurrentValue!.Login!);

            _savedServersStore.CurrentValue!.ServersAccounts = currentUserServerStore?.ServersAccounts ??
                                                                      new ObservableCollection<ServerAccount>();
        }
        catch
        {
            _savedServersStore.CurrentValue = new AppSavedServers();
        }
    }


    public async void SaveDataInFile()
    {
        try
        {
            var allUsers = new List<CurrentAccountSavedServers>();

            try
            {
                var fileText = await File.ReadAllTextAsync(FilePath);

                allUsers = JsonConvert.DeserializeObject<List<CurrentAccountSavedServers>>(fileText);

                if (allUsers!.Count != 0)
                {
                    foreach (var elem in allUsers)
                        if (elem.MainServerAccount!.Login == _accountStore.CurrentValue!.Login &&
                            elem.MainServerAccount.IsAuthorized == _accountStore.CurrentValue.IsAuthorized)

                            allUsers.Remove(elem);
                }
            }
            catch
            {
                // ignored
            }


            if (_savedServersStore.CurrentValue!.ServersAccounts?.Count != 0 &&
                _savedServersStore.CurrentValue.ServersAccounts is not null)
            {
                
                var newSavedServers = new CurrentAccountSavedServers
                {
                    LastUpdated = DateTime.Now,
                    MainServerAccount = _accountStore.CurrentValue,
                    ServersAccounts = _savedServersStore.CurrentValue.ServersAccounts
                };

                allUsers!.Add(newSavedServers);
            }


            var jsonSerializeObject = JsonConvert.SerializeObject(allUsers,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });
            
            await File.WriteAllTextAsync(FilePath, jsonSerializeObject);
        }
        catch
        {
            // ignored
        }
    }
}