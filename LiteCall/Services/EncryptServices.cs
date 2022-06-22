using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

public class EncryptServices : IEncryptServices
{
    private static readonly byte[] Entropy = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };


    public Task<string?> Sha1Encrypt(string? content)
    {
        if (string.IsNullOrEmpty(content)) return Task.FromResult<string>(null!)!;

        using var sha1 = new SHA1Managed();

        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

        return Task.FromResult(string.Concat(hash.Select(b => b.ToString("x2"))))!;
    }

    public Task<string?> Base64Encrypt(string? content)
    {
        if (string.IsNullOrEmpty(content)) return Task.FromResult(content)!;

        var originalText = Encoding.Unicode.GetBytes(content);


        var encryptedText = ProtectedData.Protect(originalText, Entropy, DataProtectionScope.CurrentUser);

        return Task.FromResult(Convert.ToBase64String(encryptedText))!;
    }

    public Task<string?> Base64Decrypt(string? content)
    {
        if (string.IsNullOrEmpty(content)) return Task.FromResult(content);

        var encryptedText = Convert.FromBase64String(content);


        var originalText = ProtectedData.Unprotect(encryptedText, Entropy, DataProtectionScope.CurrentUser);


        return Task.FromResult(Encoding.Unicode.GetString(originalText))!;
    }
}