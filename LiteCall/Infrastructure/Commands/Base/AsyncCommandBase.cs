using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiteCall.Infrastructure.Commands.Base
{
    internal abstract class AsyncCommandBase : BaseCommand
    {
        private readonly Action<Exception> _onException;

        private bool _isExecuting;
        public bool IsExecuting
        {
            get
            {
                return _isExecuting;
            }
            set
            {
                _isExecuting = value;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        public AsyncCommandBase(Action<Exception> onException)
        {
            _onException = onException;
        }

        public override bool CanExecute(object parameter)
        {
            return !IsExecuting;
        }

        public override async void Execute(object parameter)
        {
            IsExecuting = true;

            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }

            IsExecuting = false;
            CommandManager.InvalidateRequerySuggested();
        }

        protected abstract Task ExecuteAsync(object parameter);
    }
}
