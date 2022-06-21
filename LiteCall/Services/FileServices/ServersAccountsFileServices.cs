using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using Newtonsoft.Json;

namespace LiteCall.Services;

internal class ServersAccountsFileServices : IFileReadServices
{
    private const string _FilePath = @"SavedServersAccounts.json";

    private readonly AccountStore _accountStore;


    private readonly SavedServersStore _savedServersStore;

    public ServersAccountsFileServices(SavedServersStore savedServersStore, AccountStore accountStore)
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
            string? FileText = await File.ReadAllTextAsync(_FilePath);

            var AllUsers = JsonConvert.DeserializeObject<List<SavedServers>>(FileText);

            var currentUserServerStore = AllUsers.Find(s =>
                s.MainServerAccount.IsAuthorise == _accountStore.CurrentAccount.IsAuthorise &&
                s.MainServerAccount.Login == _accountStore.CurrentAccount.Login);

            _savedServersStore.SavedServerAccounts!.ServersAccounts = currentUserServerStore?.ServersAccounts ?? new ObservableCollection<ServerAccount>();
        }
        catch (Exception e)
        {
            _savedServersStore.SavedServerAccounts = new AppSavedServers();
        }
    }


    public async void SaveDataInFile()
    {
        try
        {
            var fileText = string.Empty;

            var allUsers = new List<SavedServers>();

            try
            {
                fileText = await File.ReadAllTextAsync(_FilePath);

                allUsers = JsonConvert.DeserializeObject<List<SavedServers>>(fileText);

                if (allUsers != null)
                {
                    foreach (var elem in allUsers)
                        if (elem.MainServerAccount.Login == _accountStore.CurrentAccount.Login &&
                            (elem.MainServerAccount.IsAuthorise == _accountStore.CurrentAccount.IsAuthorise))

                            allUsers.Remove(elem);
                }
                else if (allUsers == null)
                {
                    allUsers = new List<SavedServers>();
                }
               
            }
            catch (Exception e)
            {
               
            }


            if (_savedServersStore.SavedServerAccounts.ServersAccounts?.Count !=0 && _savedServersStore.SavedServerAccounts.ServersAccounts is not null)
            {
                foreach (var elem in _savedServersStore?.SavedServerAccounts?.ServersAccounts) elem.Account.Token = null;

                var newSavedServers = new SavedServers
                {
                    LastUpdated = DateTime.Now,
                    MainServerAccount = _accountStore.CurrentAccount,
                    ServersAccounts = _savedServersStore.SavedServerAccounts.ServersAccounts
                };

                allUsers.Add(newSavedServers);
            }


            var jsonSerializeObject = JsonConvert.SerializeObject(allUsers,
                new JsonSerializerSettings
                    { NullValueHandling = (NullValueHandling)1, Formatting = (Formatting)1 });


            await File.WriteAllTextAsync(_FilePath, jsonSerializeObject);
         }
        catch (Exception e)
        {
        }
    }
}