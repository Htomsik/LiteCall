using Core.Models.AccountManagement;
using Core.Models.Images;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;

namespace Core.Services.Interfaces.Connections;

public interface IHttpDataSc
{
    public Task<string> GetAuthorizeToken(RegistrationUser? newAcc, string? apiServerIp = null);

    public Task<string> Registration(RegistrationModel? registrationModel, string? apiServerIp = null);

    public Task<string?> MainServerGetApiIp(string? serverName);

    public Task<Server?> ApiServerGetInfo(string? apiServerIp);

    public Task<ImagePacket?> GetCaptcha(string? serverIp = null);

    public Task<bool> CheckServerStatus(string? serverAddress);

    public Task<string> GetRoleFromJwtToken(string token);

    public Task<List<Question>?> GetPasswordRecoveryQuestions(string? apiServerIp = null);

    public Task<bool> PasswordRecovery(RecoveryModel recoveryModel, string? apiIp = null);

    public Task<bool> PostSaveServersUserOnMainServer(Account? currentAccount, AppSavedServers savedServerAccounts);

    public Task<AppSavedServers> GetSaveServersUserOnMainServer(Account? currentAccount, DateTime? dateSynch);
}