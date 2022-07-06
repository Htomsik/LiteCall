using Core.Models.AccountManagement;

namespace Core.Services.Interfaces.AccountManagement;

public interface IGetRecoveryQuestionsSc
{
    public Task<List<Question>?> GetQuestions();
}