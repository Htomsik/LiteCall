using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Stores;

namespace LiteCall.Services.Interfaces
{
    internal interface IhttpDataServices
    {
        public Task<string> GetAuthorizeToken(Reg_Rec_PasswordAccount newAcc, string apiServerIp = null);

        public Task<string> Registration(RegistrationModel registrationModel, string apiServerIp = null);

        public Task<string> MainServerGetApiIp(string serverName);

        public Task<Server?> ApiServerGetInfo(string apiServerIp);

        public Task<ImagePacket?> GetCaptcha(string? serverIp = null);

        public Task<bool> CheckServerStatus(string serverIp);

        public Task<string> GetRoleFromJwtToken(string token);

        public Task<List<Question>?> GetPasswordRecoveryQestions(string? apiServerIp = null);

        public Task<bool> PasswordRecovery(RecoveryModel recoveryModel, string? apiIp= null);

        public Task<bool> PostSaveServersUserOnMainServer(Account currentAccount, AppSavedServers savedServerAccounts);

        public Task<AppSavedServers> GetSaveServersUserOnMainServer(Account currentAccount, DateTime? dateSynch);
    }
}
