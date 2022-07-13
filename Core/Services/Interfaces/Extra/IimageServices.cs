using Core.Models.Images;

namespace Core.Services.Interfaces.Extra;

public interface IImageServices
{
    public byte[]? GetRawData(ImagePacket? image);
}