using Core.Models.AccountManagement;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Connections;

namespace Core.Services.AccountManagement.PasswordRecovery.Questions;

public sealed class MainServerGetRecQuestionsSc : IGetRecoveryQuestionsSc
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