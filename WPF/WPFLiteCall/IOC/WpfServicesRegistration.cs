using System;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Retranslators.Base;
using Core.VMD.Base;
using LiteCall.Services;
using LiteCall.Services.Retranslators;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.IOC;

public static class WpfServicesRegistration
{
    public static IServiceCollection RegisterWpfServices(this IServiceCollection serviceCollection)
        => serviceCollection.AddTransient<ICloseAppSc, CloseAppSc>()
            .AddSingleton<IRetranslor<Type,BaseVmd>,IocHostVmdRetranslator>();

}