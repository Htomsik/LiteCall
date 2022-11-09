using Core.Services.AppInfrastructure;
using Core.Services.AppInfrastructure.FileServices;
using Core.Services.AppInfrastructure.NavigationServices.CloseServices;
using Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie;
using Core.Services.AppInfrastructure.NavigationServices.ParamsNavigationServie.Base;
using Core.Services.Connections;
using Core.Services.Extra;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Connections;
using Core.Services.Interfaces.Extra;
using Core.Stores.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.IOC;

public static class ServicesRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Сервисы
        
        #region FileServices

        services.AddSingleton<MainAccountFileService>()
            .AddSingleton<SavedServersFIleService>();
        
        #endregion

        services.AddTransient<CloseAdditionalNavigationServices>()
            .AddTransient<CloseModalNavigationServices>()
            .AddSingleton<IStatusSc, AppExecutionStateSc>()
            .AddTransient<IEncryptSc, EncryptSc>()
            .AddTransient<IImageServices, ImageSc>()
            .AddTransient<IChatServerSc, ChatServerSc>()
            .AddTransient<BaseIocTypeNavigationService>()
            .AddTransient<SettingsVmdsIocTypeNavigationService>();

        services.AddSingleton<IHttpDataSc, HttpDataSc>(s =>
            new HttpDataSc(s.GetRequiredService<IStatusSc>(), s.GetRequiredService<IEncryptSc>(),
                configuration, s.GetRequiredService<HttpClientStore>()));
        
     //   services.AddTransient<ISyncDataOnServerSc, SynchronizeDataOnServerSc>();
     
        #endregion

        return services;
    }
}