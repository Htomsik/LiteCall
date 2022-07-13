namespace Core.Services.Interfaces.AccountManagement;

public interface IGetCaptchaSc
{
    public Task<byte[]?> GetCaptcha();
}