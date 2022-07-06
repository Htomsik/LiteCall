namespace Core.Models.AppInfrastructure.StateStatuses;

public enum ExecutionActionStates:byte
{
    ServerConnection = 0,
    GettingInfoAboutServer = 1,
    CheckingServerStatus = 2,
    GettingCaptcha = 3
}