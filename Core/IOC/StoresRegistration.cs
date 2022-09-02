using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.Connections;
using Core.Stores.TemporaryInfo;
using Microsoft.Extensions.DependencyInjection;

namespace Core.IOC;

public static class StoresRegistration
{
    public static IServiceCollection RegisterStores(this IServiceCollection services)
    {
        #region Хранилища

        services.AddSingleton<MainAccountStore>();

        services.AddSingleton<AppSettingsStore>();

        services.AddSingleton<CurrentServerAccountStore>();

        services.AddSingleton<SavedServersStore>();

        services.AddSingleton<CurrentServerStore>();

        services.AddSingleton<AppExecutionStateStore>();

        services.AddSingleton<HttpClientStore>();

        services.AddSingleton<HubConnectionStore>();

        #endregion

        #region NavigationStores

        services.AddSingleton<MainWindowVmdNavigationStore>();

        services.AddSingleton<AdditionalVmdsNavigationStore>();

        services.AddSingleton<ModalVmdNavigationStore>();

        services.AddSingleton<SettingsAccountVmdNavigationStore>();

        services.AddSingleton<CurrentServerVmdNavigationStore>();

        #endregion

        return services;
    }
}