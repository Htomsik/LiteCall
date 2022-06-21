using System;
using System.Collections.Generic;
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


    private readonly ServersAccountsStore _serversAccountsStore;

    public ServersAccountsFileServices(ServersAccountsStore serversAccountsStore, AccountStore accountStore)
    {
        _serversAccountsStore = serversAccountsStore;

        _accountStore = accountStore;

        _serversAccountsStore.ServersAccountsChange += SaveDataInFile;

        _accountStore.CurrentAccountChange += GetDataFromFile;
    }


    public async void GetDataFromFile()
    {
        try
        {
            var FileText = await File.ReadAllTextAsync(_FilePath);

            var AllUsers = JsonConvert.DeserializeObject<List<SavedServers>>(FileText);

            var currentUserServerStore = AllUsers.Find(s =>
                s.MainServerAccount.IsAuthorise == _accountStore.CurrentAccount.IsAuthorise &&
                s.MainServerAccount.Login == _accountStore.CurrentAccount.Login);

            _serversAccountsStore.SavedServerAccounts.ServersAccounts = currentUserServerStore?.ServersAccounts;
        }
        catch (Exception e)
        {
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
                else
                {
                    allUsers = new List<SavedServers>();
                }
               
            }
            catch (Exception e)
            {
               
            }


            if (_serversAccountsStore.SavedServerAccounts.ServersAccounts?.Count !=0 && _serversAccountsStore.SavedServerAccounts.ServersAccounts is not null)
            {
                foreach (var elem in _serversAccountsStore?.SavedServerAccounts?.ServersAccounts) elem.Account.Token = null;

                var newSavedServers = new SavedServers
                {
                    LastUpdated = DateTime.Now,
                    MainServerAccount = _accountStore.CurrentAccount,
                    ServersAccounts = _serversAccountsStore.SavedServerAccounts.ServersAccounts
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