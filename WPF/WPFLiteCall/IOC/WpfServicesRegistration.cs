using Core.Services.Interfaces.AppInfrastructure;
using LiteCall.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.IOC;

public static class WpfServicesRegistration
{
    public static IServiceCollection RegisterWpfServices(this IServiceCollection serviceCollection)
        => serviceCollection.AddTransient<ICloseAppSc,CloseAppSc>();

}