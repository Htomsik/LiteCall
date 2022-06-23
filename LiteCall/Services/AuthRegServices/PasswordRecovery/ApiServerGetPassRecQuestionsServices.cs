using System.Collections.Generic;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class ApiServerGetPassRecQuestionsServices : IGetPassRecoveryQuestionsServices
{
    private readonly CurrentServerStore _currentServerStore;
    private readonly IHttpDataServices _httpDataServices;


    public ApiServerGetPassRecQuestionsServices(IHttpDataServices httpDataServices,
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