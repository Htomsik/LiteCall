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

            private readonly Func<object, bool> _CanExecute;

        public NavigationCommand(NavigationStore navigationStore, Func<TViewModel> createViewModel, Func<object, bool> CanExecute = null)
            {
                _NavigationStore = navigationStore;
                _CreateViewModel = createViewModel;

                _CanExecute = CanExecute;
            }


            public override bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;
            

            public override void Execute(object parameter)
            {
                if (!CanExecute(parameter)) return;
                _NavigationStore.MainWindowCurrentViewModel = _CreateViewModel();
            }
        }
    
}
