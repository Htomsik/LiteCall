using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services.AuthRegServices.PasswordRecovery;

internal sealed class MainServerGetRecQuestionsSc : IGetRecoveryQuestionsSc
{
    private readonly IHttpDataServices _httpDataServices;


    public MainServerGetRecQuestionsSc(IHttpDataServices httpDataServices)
    {
        _httpDataServices = httpDataServices;
    }

    public async Task<List<Question>?> GetQuestions()
    {
        return await _httpDataServices.GetPasswordRecoveryQuestions();
    }
}