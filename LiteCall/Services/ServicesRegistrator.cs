using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LiteCall.Services
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,IConfiguration configuration)
        {

            #region Сервисы





            services.AddSingleton<ServersAccountsFileServices>(s => new ServersAccountsFileServices(s.GetRequiredService<ServersAccountsStore>(),s.GetRequiredService<AccountStore>()));

            services.AddSingleton<MainAccountFileServices>(s => new MainAccountFileServices(s.GetRequiredService<AccountStore>(), s.GetRequiredService<SettingsStore>()));

            services.AddTransient<CloseAdditionalNavigationServices>();

            services.AddTransient<CloseModalNavigationServices>();

            var appsettings = new Appsettings();

            configuration.GetSection(Appsettings.AppSettings).Bind(appsettings);

            services.AddSingleton<IStatusServices,StatusServices>();

            services.AddTransient<IEncryptServices, EncryptServices>();

            services.AddTransient<IimageServices, ImageServices>();

            services.AddSingleton<IhttpDataServices,HttpDataService>( s => new HttpDataService(s.GetRequiredService<IStatusServices>(),s.GetRequiredService<IEncryptServices>(),appsettings));


            #endregion

            return services;
        }

    }
}
