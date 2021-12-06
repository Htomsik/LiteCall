using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Infrastructure.Commands
{
   
    
        internal class NavigationCommand<TViewModel> : BaseCommand
            where TViewModel : BaseVMD
        {

            private readonly INavigatonService<TViewModel> _NavigationService;



            public NavigationCommand(INavigatonService<TViewModel> navigationService)
            {
                _NavigationService = navigationService;
            }


            public override bool CanExecute(object parameter) => true;
            

            public override void Execute(object parameter)
            {
               
                _NavigationService.Navigate();
            }
        }
    
}
