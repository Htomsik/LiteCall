using System.Threading.Tasks;
using LiteCall.Model.RegistrationRecovery;

namespace LiteCall.Services.Interfaces;

public interface IRegistrationServices
{
    Task<int> Registration(RegistrationModel registrationModel);
}