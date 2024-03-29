﻿using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AccountManagement.Authorization;

public sealed class AuthorizationApiServerSc : IAuthorizationSc
{
    private readonly INavigationSc _closeModalNavigationSc;

    private readonly CurrentServerStore _currentServerStore;

    private readonly IHttpDataSc _httpDataSc;

    private readonly SavedServersStore _savedServersStore;

    public AuthorizationApiServerSc(SavedServersStore savedServersStore,
        CurrentServerStore currentServerStore, INavigationSc closeModalNavigationSc,
        IHttpDataSc httpDataSc)
    {
        _savedServersStore = savedServersStore;

        _currentServerStore = currentServerStore;

        _closeModalNavigationSc = closeModalNavigationSc;

        _httpDataSc = httpDataSc;
    }


    public async Task Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiSeverIp)
    {
        
        newAccount!.IsAuthorized = isNotAnonymousAuthorize;
        
        newAccount.Password = isNotAnonymousAuthorize ? string.Empty : newAccount.Password;
        
        try
        {
            var tokenResponse = await _httpDataSc.GetAuthorizeToken(newAccount, _currentServerStore.CurrentServer!.ApiIp);
        
            newAccount.Role = await _httpDataSc.GetRoleFromJwtToken(tokenResponse);
        
            newAccount.Token = tokenResponse;

         
        }
        catch (Exception)
        {
            throw new Exception();
        }
        
        _savedServersStore.Replace(_currentServerStore.CurrentServer, newAccount);

        _closeModalNavigationSc.Navigate();
        
    }
}