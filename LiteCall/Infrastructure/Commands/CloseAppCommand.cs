using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Infrastructure.Commands.Base;

namespace LiteCall.Infrastructure.Commands
{
    internal class CloseAppCommand:BaseCommand
    {
        public override bool CanExecute(object parameter)
        {
            return true;
        }


        public override void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
