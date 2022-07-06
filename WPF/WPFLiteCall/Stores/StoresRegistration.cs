using Core.Stores.AppInfrastructure;
using Core.Stores.AppInfrastructure.NavigationStores;
using Core.Stores.Connections;
using Core.Stores.TemporaryInfo;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Stores;

internal static class StoresRegistration
{
    public static IServiceCollection RegisterStores(this IServiceCollection services)
    {
        #region Хранилища

        services.AddSingleton<MainAccountStore, MainAccountStore>();

        services.AddSingleton<AppSettingsStore>();

        services.AddSingleton<CurrentServerAccountStore>();

        services.AddSingleton<SavedServersStore, SavedServersStore>();

        services.AddSingleton<CurrentServerStore>();

        services.AddSingleton<AppExecutionStateStore>();

        services.AddSingleton<HttpClientStore>();

        services.AddSingleton<HubConnectionStore>();

        #endregion

        #region NavigationStores

        services.AddSingleton<MainWindowNavigationStore>();

        services.AddSingleton<AdditionalNavigationStore>();

        services.AddSingleton<ModalNavigationStore>();

        services.AddSingleton<SettingsAccNavigationStore>();

        services.AddSingleton<MainPageServerNavigationStore>();

        #endregion

        return services;
    }
}