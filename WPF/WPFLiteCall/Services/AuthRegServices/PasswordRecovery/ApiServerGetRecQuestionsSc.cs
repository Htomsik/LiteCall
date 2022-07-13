using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using Core.Stores.TemporaryInfo;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class ApiServerGetRecQuestionsSc : IGetRecoveryQuestionsSc
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataSc _httpDataSc;


    public ApiServerGetRecQuestionsSc(IHttpDataSc httpDataSc,
        CurrentServerStore currentServerStore)
    {
        _httpDataSc = httpDataSc;

        _currentServerStore = currentServerStore;
    }

    public async Task<List<Question>?> GetQuestions()
    {
        return await _httpDataSc.GetPasswordRecoveryQuestions(_currentServerStore.CurrentServer!.ApiIp);
    }
}