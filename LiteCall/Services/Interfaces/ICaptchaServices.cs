using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiteCall.Model;

namespace LiteCall.Services.Interfaces
{
    internal interface ICaptchaServices
    {
        public Task<ImageSource?> GetCaptcha();
    }
}
