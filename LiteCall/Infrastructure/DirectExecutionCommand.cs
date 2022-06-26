using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands.Base;

namespace LiteCall.Infrastructure
{
    internal class DirectExecutionCommand:BaseCommand
    {
        private readonly Func<object?, Task> _execute;
        
        public DirectExecutionCommand(Func<object?, Task> execute )
        {
            _execute = execute;
        }

        public override bool CanExecute(object? parameter) => true;
        

        public  override void Execute(object? parameter)
        {
            _execute(null);
            CommandManager.InvalidateRequerySuggested();
        }

       
    }
}
