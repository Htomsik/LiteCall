using System.ComponentModel;
using System.Windows.Input;

namespace Core.VMD.AppInfrastructure.Windows.Base;

/// <summary>
///     Window interface
/// </summary>
public interface IBaseWindowVmd : INotifyPropertyChanged
{
     /// <summary>
     ///     Titile of window
     /// </summary>
     string Title { get; }

     /// <summary>
     ///     Path to window image
     /// </summary>
     string ImagePath { get; }

     /// <summary>
     ///     Close window command
     /// </summary>
     ICommand CloseWindowCommand { get; }
}