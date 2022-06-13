using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerAuthorisationModalVMD : AuthorisationPageVMD
    {
        public ServerAuthorisationModalVMD(INavigationService registrationNavigationServices, IAuthorisationServices authorisationServices) : base(registrationNavigationServices, authorisationServices)
        {

        }
    }
}
