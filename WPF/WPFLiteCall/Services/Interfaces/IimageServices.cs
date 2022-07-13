using System.Drawing;
using System.Windows.Media.Imaging;
using Core.Models.Images;

namespace LiteCall.Services.Interfaces;

public interface IImageServices
{
    public byte[]? GetRawData(ImagePacket? image);
}