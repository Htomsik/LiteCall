using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Core.Models.AppInfrastructure;

public class AppSettings:ReactiveObject
{
    [Reactive]
    public int OutputDeviceId { get; set; }
    
    [Reactive]
    public int CaptureDeviceId { get; set; }
    
}