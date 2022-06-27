using LiteCall.Model.Users;

namespace LiteCall.Model.RegistrationRecovery;

public class RecoveryModel
{
    public RegRecPasswordAccount? RecoveryAccount { get; set; }

    public Question? Question { get; set; }

    public string? QuestionAnswer { get; set; }
}