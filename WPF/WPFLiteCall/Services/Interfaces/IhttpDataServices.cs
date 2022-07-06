using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models.AccountManagement;
using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using LiteCall.Model.Images;

namespace LiteCall.Services.Interfaces;

public interface IHttpDataServices
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