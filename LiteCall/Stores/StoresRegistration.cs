using LiteCall.Stores.NavigationStores;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Stores;

internal static class StoresRegistration
{
    public static IServiceCollection RegisterStores(this IServiceCollection services)
    {
        #region Хранилища

        services.AddSingleton<AccountStore, AccountStore>();

        services.AddSingleton<SettingsStore>();

        services.AddSingleton<ServerAccountStore>();

        services.AddSingleton<SavedServersStore, SavedServersStore>();

        services.AddSingleton<CurrentServerStore>();

        services.AddSingleton<StatusMessageStore>();

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