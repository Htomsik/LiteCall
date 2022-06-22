using System.Threading.Tasks;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces;

internal interface IRecoveryPasswordServices
{
    public Task<bool> RecoveryPassword(RecoveryModel recoveryModel);
}