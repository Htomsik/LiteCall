using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Close;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.AppInfrastructure.Windows.Base;

/// <summary>
///     Base realize for IBaseWindowVmd
/// </summary>
public abstract class BaseWindowVmdVmd : BaseVmd,IBaseWindowVmd
{
    #region Constructors
    /// <param name="closeWindowService">Close current window service</param>
    public BaseWindowVmdVmd(ICloseServices closeWindowService)
    {
        CloseWindowCommand = ReactiveCommand.Create(()=> closeWindowService?.Close());
    }
    #endregion

    #region Commands

    public ICommand CloseWindowCommand { get; }

    #endregion

    #region Properties and Fields
    public string Title => "LiteCall";

    public string ImagePath => "/Resources/Assets/Icons/AppIcon.png";

    #endregion
    
}