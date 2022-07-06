namespace Core.Models.AppInfrastructure.StateStatuses;

public enum ExecutionErrorStates:byte
{
    ServerConnectionFailed = 0,
    IncorrectServerNameOrIp = 1,
    IncorrectServerIp = 2,
    UnknownError = 3,
    AuthorizationFailed = 4,
    RegistrationFailed = 5
}
