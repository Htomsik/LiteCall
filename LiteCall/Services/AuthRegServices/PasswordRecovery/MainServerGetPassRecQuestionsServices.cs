using System.Collections.Generic;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerGetPassRecQuestionsServices : IGetPassRecoveryQuestionsServices
{
    private readonly IHttpDataServices _httpDataServices;


    public MainServerGetPassRecQuestionsServices(IHttpDataServices httpDataServices)
    {
        _httpDataServices = httpDataServices;
    }

    public async Task<List<Question>?> GetQuestions()
    {
        return await _httpDataServices.GetPasswordRecoveryQuestions();
    }
}