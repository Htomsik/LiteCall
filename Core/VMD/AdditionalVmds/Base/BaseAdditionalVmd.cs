using System.ComponentModel;
using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Close;
using Core.Infrastructure.CMD;
using Core.VMD.Base;

namespace Core.VMD.AdditionalVmds.Base;

/// <summary>
///     Base abstract for IAdditionalVmd
/// </summary>
public abstract class BaseAdditionalVmd : BaseVmd, IAdditionalVmd
{
    #region Commands
    public ICommand CloseAdditionalVmdCommand { get; }

    #endregion

    #region Constructors
    
    /// <param name="closeAdditionalNavigationServices">Services that close Additional vmds</param>
    public BaseAdditionalVmd(ICloseServices closeAdditionalNavigationServices) => CloseAdditionalVmdCommand = new CloseNavigationCmd(closeAdditionalNavigationServices);

    #endregion
    
}