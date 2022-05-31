using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiteCall.Infrastructure.Commands.InterFace
{
    internal interface IAsynCommand<in T>:ICommand
    {
        IEnumerable<Task> RunningTasks { get; }
        bool CanExecute(T parameter);
        Task ExecuteAsync(T parameter);
    }
}
