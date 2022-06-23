using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LiteCall.Model;

public class ImagePacket
{
    public ImagePacket()
    {
    }

    public ImagePacket(byte[]? imgSources)
    {
        Hash = ImageBox.StringHash(imgSources);
        Len = imgSources!.Length;
        Image = ImageBox.EncodeBytes(imgSources);
    }

    private string Hash { get; } = string.Empty;
    private int Len { get; }
    private string Image { get; } = string.Empty;

    public byte[]? GetRawData()
    {
        var data = ImageBox.DecodeBytes(Image);

        if (data!.Length != Len)
        {
            return null;
        }

        if (!ImageBox.StringHash(data).Equals(Hash))
        {
            return null;
        }


        return data;
    }
}

public static class ImageBox
{
    public static Image TakeScreen()
    {
        var bitmap = new Bitmap(110, 50);
        var g = Graphics.FromImage(bitmap);
        g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
        return bitmap;
    }

    public static byte[] ImageToBytes(Image value)
    {
        var converter = new ImageConverter();
        var arr = (byte[])converter.ConvertTo(value, typeof(byte[]))!;
        return arr;
    }


    public static Image BytesToImage(byte[]? value)
    {
        using (var ms = new MemoryStream(value))
        {
            return Image.FromStream(ms);
        }
    }

    public static string EncodeBytes(byte[]? value)
    {
        return Convert.ToBase64String(value!);
    }

    public static byte[]? DecodeBytes(string value)
    {
        return Convert.FromBase64String(value);
    }

    public static string StringHash(byte[]? value)
    {
        using (var md5 = MD5.Create())
        {
            var hashBytes = md5.ComputeHash(value!);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
                sb.Append(t.ToString("X2"));

            return sb.ToString().ToLower();
        }
    }
}