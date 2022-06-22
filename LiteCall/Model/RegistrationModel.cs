namespace LiteCall.Model;

public class RegistrationModel : RecoveryModel
{
    public string? Captcha { get; set; }
}

public class RecoveryModel
{
    public RegRecPasswordAccount? RecoveryAccount { get; set; }

    public Question? Question { get; set; }

    public string? QestionAnswer { get; set; }
}