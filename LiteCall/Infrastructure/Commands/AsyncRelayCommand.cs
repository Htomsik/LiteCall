using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.Base;

namespace LiteCall.Infrastructure.Commands
{
    internal class AsyncRelayCommand : AsyncCommandBase
    {

        private readonly Func<object,Task> _Execute;

        private readonly Func<object, bool> _CanExecute;

        public AsyncRelayCommand(Func<object, Task> Execute, Action<Exception> onException, Func<object, bool> CanExecute = null) : base(onException)
        {
            _Execute = Execute;
            _CanExecute = CanExecute;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _Execute(parameter);
        }

        public override bool CanExecute(object parameter)
        {
            if (!IsExecuting)
            {
                return (_CanExecute?.Invoke(parameter) ?? true);
            }
            else
            {
                return false;
            }

        }
    }
}
