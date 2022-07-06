namespace Core.Services.Interfaces.Extra;

public interface IEncryptSc
{
    public Task<string?> Sha1Encrypt(string? content);

    public Task<string?> Base64Encrypt(string? content);

    public Task<string?> Base64Decrypt(string? content);
}