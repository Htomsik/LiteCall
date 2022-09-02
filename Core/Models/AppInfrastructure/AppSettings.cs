using Core.VMD.Base;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.AppInfrastructure;

public class AppSettings:BaseVmd
{
    [Reactive]
    public int OutputDeviceId { get; set; }
    
    [Reactive]
    public int CaptureDeviceId { get; set; }
    
}