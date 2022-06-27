namespace LiteCall.Model.Statuses;

public enum StatusesActions : byte
{
    ServerConnection = 0,
    GettingInfoAboutServer = 1,
    CheckingServerStatus = 2,
    GettingCaptcha = 3,
}