


using Core.Models.AccountManagement;

namespace Core.Services.Interfaces.AccountManagement;

public interface IRecoveryPasswordSc
{
    public Task<bool> RecoveryPassword(RecoveryModel recoveryModel);
}