﻿using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Model.Users;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.Authorization;

internal sealed class AuthorizationMainServerServices : IAuthorizationServices
{
    private readonly IHttpDataServices _httpDataServices;

    private readonly AccountStore _mainAccountStore;

    private readonly IStatusServices _statusServices;

    public AuthorizationMainServerServices(AccountStore accountStore, IHttpDataServices httpDataServices,
        IStatusServices statusServices)
    {
        _mainAccountStore = accountStore;

        _httpDataServices = httpDataServices;

        _statusServices = statusServices;
    }

    public async Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? severIp = null)
    {
        if (isNotAnonymousAuthorize)
        {
            var response = await _httpDataServices.GetAuthorizeToken(newAccount);

            if (response == "invalid") return 0;

            newAccount!.Token = response;

            newAccount.IsAuthorized = true;
        }
        else
        {
            newAccount!.IsAuthorized = false;

            newAccount.Password = "";
        }

        _mainAccountStore.CurrentAccount = newAccount;

        return 1;
    }
}