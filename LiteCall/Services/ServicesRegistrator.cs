using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.Services
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection RegisterrServices(this IServiceCollection services)
        {

            #region Сервисы

            services.AddSingleton<ServersAccountsFileServices>(s => new ServersAccountsFileServices(s.GetRequiredService<ServersAccountsStore>()));

            services.AddSingleton<MainAccountFileServices>(s => new MainAccountFileServices(s.GetRequiredService<AccountStore>(), s.GetRequiredService<SettingsStore>()));

            services.AddSingleton<CloseAdditionalNavigationServices>();

            services.AddSingleton<CloseModalNavigationServices>();

            services.AddSingleton<IStatusServices,StatusServices>();

            services.AddSingleton<IEncryptServices, EncryptServices>();

            services.AddSingleton<IimageServices, ImageServices>();

            services.AddSingleton<IhttpDataServices, HttpDataService>();

            #endregion

            return services;
        }
    }
}
