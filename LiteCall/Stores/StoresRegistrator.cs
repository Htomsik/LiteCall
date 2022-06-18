using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Stores
{
    internal static class StoresRegistrator
    {
        public static IServiceCollection RegisterStores(this IServiceCollection services)
        {
            #region Хранилища

            services.AddSingleton<AccountStore>();

            services.AddSingleton<SettingsStore>();

            services.AddSingleton<ServerAccountStore>();

            services.AddSingleton<ServersAccountsStore>();

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
}
