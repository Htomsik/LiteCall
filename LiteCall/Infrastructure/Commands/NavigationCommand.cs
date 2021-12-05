using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Services;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Infrastructure.Commands
{
   
    
        internal class NavigationCommand<TViewModel> : BaseCommand
            where TViewModel : BaseVMD
        {

            private readonly NavigationServices<TViewModel> _NavigationService;



            public NavigationCommand(NavigationServices<TViewModel> navigationService)
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
