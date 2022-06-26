using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Model.Images;
using LiteCall.Model.RegistrationRecovery;
using LiteCall.Model.Saved;
using LiteCall.Model.ServerModels;
using LiteCall.Model.Users;

namespace LiteCall.Services.Interfaces;

internal interface IHttpDataServices
{
    public Task<string> GetAuthorizeToken(RegRecPasswordAccount? newAcc, string? apiServerIp = null);

    public Task<string> Registration(RegistrationModel registrationModel, string? apiServerIp = null);

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