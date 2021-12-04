using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.Infrastructure.Commands
{
   
    
        internal class NavigationCommand<TViewModel> : BaseCommand
            where TViewModel : BaseVMD
        {
            private readonly NavigationStore _NavigationStore;
            private readonly Func<TViewModel> _CreateViewModel;

            public NavigationCommand(NavigationStore navigationStore, Func<TViewModel> createViewModel)
            {
                _NavigationStore = navigationStore;
                _CreateViewModel = createViewModel;
            }


            public override bool CanExecute(object parameter)
            {
                return true;
            }

            public override void Execute(object parameter)
            {
                _NavigationStore.MainWindowCurrentViewModel = _CreateViewModel();
            }
        }
    
}
