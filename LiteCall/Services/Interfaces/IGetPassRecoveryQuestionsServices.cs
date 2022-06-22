using System.Collections.Generic;
using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces;

public interface IGetPassRecoveryQuestionsServices
{
    public Task<List<Question>?> GetQuestions();
}