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
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {

            #region Сервисы

            services.AddSingleton<ServersAccountsFileServices>(s => new ServersAccountsFileServices(s.GetRequiredService<ServersAccountsStore>()));

            services.AddSingleton<MainAccountFileServices>(s => new MainAccountFileServices(s.GetRequiredService<AccountStore>(), s.GetRequiredService<SettingsStore>()));

            services.AddTransient<CloseAdditionalNavigationServices>();

            services.AddTransient<CloseModalNavigationServices>();

            services.AddSingleton<IStatusServices,StatusServices>();

            services.AddTransient<IEncryptServices, EncryptServices>();

            services.AddTransient<IimageServices, ImageServices>();

            services.AddSingleton<IhttpDataServices, HttpDataService>();

            #endregion

            return services;
        }
    }
}
