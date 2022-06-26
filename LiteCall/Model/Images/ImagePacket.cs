namespace LiteCall.Model.Images;

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

        if (data!.Length != Len) return null;

        if (!ImageBox.StringHash(data).Equals(Hash)) return null;


        return data;
    }
}