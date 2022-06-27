using System.Threading.Tasks;
using LiteCall.Model.Users;

namespace LiteCall.Services.Interfaces;

public interface IAuthorizationServices
{
    Task<int> Login(bool isNotAnonymousAuthorize, Account? newAccount, string? apiServerIp = null);
}