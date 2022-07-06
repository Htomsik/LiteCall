using Core.Models.AccountManagement;

namespace Core.Services.Interfaces.AccountManagement;

public interface IRegistrationSc
{
    Task<int> Registration(RegistrationModel registrationModel);
}