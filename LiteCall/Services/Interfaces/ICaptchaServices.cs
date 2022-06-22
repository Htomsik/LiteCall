using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteCall.Services.Interfaces;

internal interface ICaptchaServices
{
    public Task<ImageSource?> GetCaptcha();
}