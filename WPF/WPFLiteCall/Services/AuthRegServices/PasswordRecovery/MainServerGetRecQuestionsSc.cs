using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerGetRecQuestionsSc : IGetRecoveryQuestionsSc
{
    private readonly IHttpDataSc _httpDataSc;


    public MainServerGetRecQuestionsSc(IHttpDataSc httpDataSc)
    {
        _httpDataSc = httpDataSc;
    }

    public async Task<List<Question>?> GetQuestions()
    {
        return await _httpDataSc.GetPasswordRecoveryQuestions();
    }
}