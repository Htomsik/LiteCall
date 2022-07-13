using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core.Models.Images;
using LiteCall.Services.Interfaces;

namespace LiteCall.Services;

public class ImageServices : IImageServices
{
    
    public byte[]? GetRawData(ImagePacket? image)
    {
        var data = DecodeBytes(image!.Bytes);

        if (data!.Length != image.Lenght) return null;

        return !StringHash(data).Equals(image.Hash) ? null : data;
    }
    
    private byte[]? DecodeBytes(string value)
    {
        return Convert.FromBase64String(value);
    }

    private string StringHash(byte[]? value)
    {
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(value!);
        var sb = new StringBuilder();
        foreach (var t in hashBytes)
            sb.Append(t.ToString("X2"));

        return sb.ToString().ToLower();
    }
}