﻿using Core.Models.Users;
using Core.Services.AppInfrastructure.FileServices.Base;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AppInfrastructure.FileServices;

/// <summary>
///     File Service for main account
/// </summary>
public sealed class MainAccountFileService : DataFileService<Account>
{
    public MainAccountFileService(MainAccountStore mainAccountStore) : base(mainAccountStore, mainAccountStore)
    {
    }

    protected override string FileName => "Account.json";
}