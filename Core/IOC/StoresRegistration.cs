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
        #region Stores

        services
            .AddSingleton<MainAccountStore>()
            .AddSingleton<AppSettingsStore>()
            .AddSingleton<CurrentServerAccountStore>()
            .AddSingleton<SavedServersStore>()
            .AddSingleton<CurrentServerStore>()
            .AddSingleton<AppExecutionStateStore>()
            .AddSingleton<HttpClientStore>()
            .AddSingleton<HubConnectionStore>()
            .AddSingleton<SettingsVmdsNavigationStore>()
            .AddSingleton<SettingsAccountManagementVmdNavigationStore>();
        #endregion

        #region NavigationStores

        services
            .AddSingleton<MainWindowVmdNavigationStore>()
            .AddSingleton<AdditionalVmdsNavigationStore>()
            .AddSingleton<ModalVmdNavigationStore>()
            .AddSingleton<SettingsAccountManagementVmdNavigationStore>()
            .AddSingleton<CurrentServerVmdNavigationStore>();

        #endregion

        return services;
    }
}