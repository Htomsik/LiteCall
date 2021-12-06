using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Services.Interfaces
{
    internal interface INavigatonService<TViewModel> where TViewModel : BaseVMD
    {
        void Navigate();
    }
}
