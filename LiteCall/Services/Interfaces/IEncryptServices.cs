using System.Threading.Tasks;

namespace LiteCall.Services.Interfaces;

public interface IEncryptServices
{
    public Task<string?> Sha1Encrypt(string? content);

    public Task<string?> Base64Encrypt(string? content);

    public Task<string?> Base64Decrypt(string? content);
}