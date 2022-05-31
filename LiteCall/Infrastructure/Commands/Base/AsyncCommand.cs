using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using LiteCall.Infrastructure.Commands.InterFace;

namespace LiteCall.Infrastructure.Commands.Base
{
    public abstract class AsyncCommand<T> : IAsynCommand<T>
    {
        private readonly ObservableCollection<Task> runningTasks;

       
        protected AsyncCommand()
        {
            runningTasks = new ObservableCollection<Task>();
            runningTasks.CollectionChanged += OnRunningTasksChanged;
        }

  
        public IEnumerable<Task> RunningTasks
        {
            get => runningTasks;
        }

       
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

      
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

     
        public abstract bool CanExecute(T parameter);

    




        async void ICommand.Execute(object parameter)
        {
            Task runningTask = ExecuteAsync((T)parameter);

            runningTasks.Add(runningTask);

            try
            {
                await runningTask.ConfigureAwait(false);
            }
            finally
            {
                runningTasks.Remove(runningTask);
            }
        }

     
        public abstract Task ExecuteAsync(T parameter);

        private void OnRunningTasksChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
