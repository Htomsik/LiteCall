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

        _savedServersStore.ServersAccountsChange += SaveDataInFile;

        _accountStore.CurrentAccountChange += GetDataFromFile;
    }


    public async void GetDataFromFile()
    {
        try
        {
            var fileText = await File.ReadAllTextAsync(FilePath);

            var allUsers = JsonConvert.DeserializeObject<List<CurrentAccountSavedServers>>(fileText);

            var currentUserServerStore = allUsers!.Find(s =>
                s.MainServerAccount!.IsAuthorized! == _accountStore!.CurrentAccount!.IsAuthorized! &&
                s!.MainServerAccount!.Login! == _accountStore!.CurrentAccount!.Login!);

            _savedServersStore.SavedServerAccounts!.ServersAccounts = currentUserServerStore?.ServersAccounts ??
                                                                      new ObservableCollection<ServerAccount>();
        }
        catch
        {
            _savedServersStore.SavedServerAccounts = new AppSavedServers();
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
                        if (elem.MainServerAccount!.Login == _accountStore.CurrentAccount!.Login &&
                            elem.MainServerAccount.IsAuthorized == _accountStore.CurrentAccount.IsAuthorized)

                            allUsers.Remove(elem);
                }
            }
            catch
            {
                // ignored
            }


            if (_savedServersStore.SavedServerAccounts!.ServersAccounts?.Count != 0 &&
                _savedServersStore.SavedServerAccounts.ServersAccounts is not null)
            {
                
                var newSavedServers = new CurrentAccountSavedServers
                {
                    LastUpdated = DateTime.Now,
                    MainServerAccount = _accountStore.CurrentAccount,
                    ServersAccounts = _savedServersStore.SavedServerAccounts.ServersAccounts
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