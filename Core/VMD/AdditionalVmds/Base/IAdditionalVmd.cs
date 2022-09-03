using System.ComponentModel;
using System.Windows.Input;

namespace Core.VMD.AdditionalVmds.Base;

/// <summary>
///     Additional vmd
/// </summary>
public interface IAdditionalVmd : INotifyPropertyChanged
{
    /// <summary>
    ///     Close additional vmd command
    /// </summary>
    ICommand CloseAdditionalVmdCommand { get; }
}