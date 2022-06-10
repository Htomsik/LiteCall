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
   
    
        internal class NavigationCommand : BaseCommand
           
        {

            private readonly INavigationService _NavigationService;

            private readonly Func<bool> _CanExecute;

        public NavigationCommand(INavigationService navigationService, Func<bool> CanExecute = null)
            {
                _NavigationService = navigationService;
                _CanExecute = CanExecute;
        }


        public override bool CanExecute(object parameter) => _CanExecute?.Invoke() ?? true;


        public override void Execute(object parameter)
            {
               
                _NavigationService.Navigate();
            }
        }
    
}
