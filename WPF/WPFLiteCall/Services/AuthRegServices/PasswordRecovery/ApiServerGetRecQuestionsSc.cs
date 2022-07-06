using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class ApiServerGetRecQuestionsSc : IGetRecoveryQuestionsSc
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataServices _httpDataServices;


    public ApiServerGetRecQuestionsSc(IHttpDataServices httpDataServices,
        CurrentServerStore currentServerStore)
    {
        _httpDataServices = httpDataServices;

        _currentServerStore = currentServerStore;
    }

    public async Task<List<Question>?> GetQuestions()
    {
        return await _httpDataServices.GetPasswordRecoveryQuestions(_currentServerStore.CurrentServer!.ApiIp);
    }
}