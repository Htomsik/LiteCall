using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Services;
using LiteCall.Services.Authorisation;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.ServerPages;
using Microsoft.Extensions.DependencyInjection;

namespace LiteCall.ViewModels
{
    internal static class VMDRegistrator
    {
      public  static IServiceCollection RegisterVMD(this IServiceCollection services)
      {


            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            #region Регистрация/Авторизация на мейн сервере

            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD(
                CreateRegistrationPageNavigationServices(s), CreateMainAuthorisationServices(s)));


            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                s.GetRequiredService<AccountStore>(),
                CreateAutPageNavigationServices(s), CreateMainRegistrationSevices(s)));

            #endregion

            #region Регистрация/Авторизация на API сервере чата

            services.AddTransient<ServerRegistrationModalVMD>(s => new ServerRegistrationModalVMD(
                CreateModalAuthorisationPageNavigationService(s),
                CreateApiRegistrationSevices(s),
                s.GetRequiredService<CurrentServerStore>()));


            services.AddTransient<ServerAuthorisationModalVMD>(s => new ServerAuthorisationModalVMD(
                CreateModalRegistrationPageNavigationServices(s), CreateApiAuthorisationServices(s)));


            #endregion

            #region Остальные VMD

            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<ServersAccountsStore>(),
                    s.GetRequiredService<CurrentServerStore>(),
                    s.GetRequiredService<MainPageServerNavigationStore>(),
                    CreateSettingPageNavigationService(s),
                    CreateServerPageNavigationService(s),
                    CreateModalAuthorisationPageNavigationService(s),
                    CreateAuthCheckApiServerSevices(s),
                    s.GetRequiredService<IStatusServices>()));

            services.AddTransient<SettingVMD>(s => new SettingVMD(s.GetRequiredService<AccountStore>(), s.GetRequiredService<ServersAccountsStore>(),
                CreateAutPageNavigationServices(s), s.GetRequiredService<SettingsAccNavigationStore>()
            ));


            services.AddTransient<ServerVMD>(s =>
                new ServerVMD(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>(),s.GetRequiredService<IStatusServices>()));



            services.AddSingleton<MainWindowVMD>(s => new MainWindowVMD(
                s.GetRequiredService<MainWindowNavigationStore>(), s.GetRequiredService<AdditionalNavigationStore>(),
                s.GetRequiredService<ModalNavigationStore>(),
                s.GetRequiredService<StatusMessageStore>(),
                s.GetRequiredService<CloseModalNavigationServices>(),
                s.GetRequiredService<CloseAdditionalNavigationServices>()));

            #endregion

            return services;
      }

        #region Создание навигационных сервисов для базовых VMD

        private static INavigationService CreateAutPageNavigationServices(IServiceProvider serviceProvider)
        {

            return new SettingAccNavigationServices<AuthorisationPageVMD>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<AuthorisationPageVMD>());
        }

        private static INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<RegistrationPageVMD>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<RegistrationPageVMD>());
        }

        private static INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<MainPageVMD>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
                () => serviceProvider.GetRequiredService<MainPageVMD>());
        }

        private static INavigationService CreateSettingPageNavigationService(IServiceProvider serviceProvider)
        {
            return new AdditionalNavigationServices<SettingVMD>(serviceProvider.GetRequiredService<AdditionalNavigationStore>(), () => serviceProvider.GetRequiredService<SettingVMD>());
        }

        private static INavigationService CreateServerPageNavigationService(IServiceProvider serviceProvider)
        {
            return new MainPageServerNavigationSevices<ServerVMD>(serviceProvider.GetRequiredService<MainPageServerNavigationStore>(), () => serviceProvider.GetRequiredService<ServerVMD>());
        }

        #endregion

        #region Модальные окна VMD

        private static INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerRegistrationModalVMD>
            (serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ServerRegistrationModalVMD>());
        }

        private static INavigationService CreateModalAuthorisationPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerAuthorisationModalVMD>(serviceProvider.GetRequiredService<ModalNavigationStore>(), () => serviceProvider.GetRequiredService<ServerAuthorisationModalVMD>());
        }


        #endregion

        #region Авторизация/Регистрация VMD
        private static IRegistrationSevices CreateApiRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(), serviceProvider.GetRequiredService<CloseModalNavigationServices>());
        }

        private static IRegistrationSevices CreateMainRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationMainServerService(serviceProvider.GetRequiredService<AccountStore>());
        }



        private static IAuthorisationServices CreateApiAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthorisationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(),
                serviceProvider.GetRequiredService<CloseModalNavigationServices>());
        }

        private static IAuthorisationServices CreateMainAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthoisationMainServerServices(serviceProvider.GetRequiredService<AccountStore>());
        }

        private static IAuthorisationServices CreateAuthCheckApiServerSevices(IServiceProvider serviceProvider)
        {
            return new AuthCheckApiServerSevices(serviceProvider.GetRequiredService<ServerAccountStore>());
        }

        #endregion
    }
}
