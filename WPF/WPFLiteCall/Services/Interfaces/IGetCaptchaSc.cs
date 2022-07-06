using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteCall.Services.Interfaces;

internal interface IGetCaptchaSc
{
    public Task<ImageSource?> GetCaptcha();
}