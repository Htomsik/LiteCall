using LiteCall.Services.FileServices;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Services;

internal static class ServicesRegistration
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        #region Сервисы

        services.AddSingleton(s =>
            new ServersAccountsFileServices(s.GetRequiredService<SavedServersStore>(),
                s.GetRequiredService<AccountStore>()));

        services.AddSingleton(s =>
            new MainAccountFileServices(s.GetRequiredService<AccountStore>(), s.GetRequiredService<SettingsStore>()));

        services.AddTransient<CloseAdditionalNavigationServices>();

        services.AddTransient<CloseModalNavigationServices>();

        services.AddSingleton<IStatusServices, StatusServices>();

        services.AddTransient<IEncryptServices, EncryptServices>();

        services.AddTransient<IImageServices, ImageServices>();

        services.AddSingleton<IHttpDataServices, HttpDataService>(s =>
            new HttpDataService(s.GetRequiredService<IStatusServices>(), s.GetRequiredService<IEncryptServices>(),
                configuration));

        services.AddTransient<ICloseAppServices, CloseAppServices>();

        services.AddTransient<ISynhronyzeDataOnServerServices, SynchronizeDataOnServerServices>();

        #endregion

        return services;
    }
}